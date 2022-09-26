using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{

    [AddComponentMenu("FMOD Studio/FMOD Studio Event Emitter")]
    public class StudioEventEmitter : EventHandler
    {

        public EventReference EventReference;

        [Obsolete("Use the EventReference field instead")]
        public string Event = "";

        public EmitterGameEvent PlayEvent = EmitterGameEvent.None;
        public EmitterGameEvent StopEvent = EmitterGameEvent.None;
        public bool AllowFadeout = true;
        public bool TriggerOnce = false;
        public bool Preload = false;
        public ParamRef[] Params = new ParamRef[0];
        public bool OverrideAttenuation = false;
        public float OverrideMinDistance = -1.0f;
        public float OverrideMaxDistance = -1.0f;

        protected FMOD.Studio.EventDescription eventDescription;

        protected FMOD.Studio.EventInstance instance;

        private bool hasTriggered = false;
        private bool isQuitting = false;
        private bool isOneshot = false;
        private List<ParamRef> cachedParams = new List<ParamRef>();

        private const string SnapshotString = "snapshot";

        public FMOD.Studio.EventDescription EventDescription { get { return eventDescription; } }

        public FMOD.Studio.EventInstance EventInstance { get { return instance; } }

        public bool IsActive { get; private set; }

        #region Autoculling
        public bool occlusionEnabled = false;
        public string occlusionParameterName = "Occlusion";
        [Range(0.0f, 10.0f)]
        public float occlusionIntensity = 1f;
        float nextOcclusionUpdate = 0.0f;

        float currentOcclusion_Final = 0.0f;
        float currentOcclusion_Target = 0.0f;
        float currentOcclusion_Calculation = 0.0f;
        #endregion

        public float MaxDistance
        {
            get
            {
                if (OverrideAttenuation)
                {
                    return OverrideMaxDistance;
                }

                if (!eventDescription.isValid())
                {
                    Lookup();
                }

                float minDistance, maxDistance;
                eventDescription.getMinMaxDistance(out minDistance, out maxDistance);
                return maxDistance;
            }
        }

        protected override void Start()
        {
            RuntimeUtils.EnforceLibraryOrder();
            if (Preload)
            {
                Lookup();
                eventDescription.loadSampleData();
            }

            HandleGameEvent(EmitterGameEvent.ObjectStart);
        }

        void OnApplicationQuit()
        {
            isQuitting = true;
        }

        protected override void OnDestroy()
        {
            if (!isQuitting)
            {
                HandleGameEvent(EmitterGameEvent.ObjectDestroy);

                if (instance.isValid())
                {
                    RuntimeManager.DetachInstanceFromGameObject(instance);
                    if (eventDescription.isValid() && isOneshot)
                    {
                        instance.release();
                        instance.clearHandle();
                    }
                }

                RuntimeManager.DeregisterActiveEmitter(this);

                if (Preload)
                {
                    eventDescription.unloadSampleData();
                }
            }
        }

        protected override void HandleGameEvent(EmitterGameEvent gameEvent)
        {
            if (PlayEvent == gameEvent)
            {
                Play();
            }
            if (StopEvent == gameEvent)
            {
                Stop();
            }
        }

        void Lookup()
        {
            eventDescription = RuntimeManager.GetEventDescription(EventReference);

            if (eventDescription.isValid())
            {
                for (int i = 0; i < Params.Length; i++)
                {
                    FMOD.Studio.PARAMETER_DESCRIPTION param;
                    eventDescription.getParameterDescriptionByName(Params[i].Name, out param);
                    Params[i].ID = param.id;
                }
            }
        }

        public void Play()
        {
            if (TriggerOnce && hasTriggered)
            {
                return;
            }

            if (EventReference.IsNull)
            {
                return;
            }

            cachedParams.Clear();

            if (!eventDescription.isValid())
            {
                Lookup();
            }

            bool isSnapshot;
            eventDescription.isSnapshot(out isSnapshot);

            if (!isSnapshot)
            {
                eventDescription.isOneshot(out isOneshot);
            }

            bool is3D;
            eventDescription.is3D(out is3D);

            IsActive = true;

            if (is3D && !isOneshot && Settings.Instance.StopEventsOutsideMaxDistance)
            {
                RuntimeManager.RegisterActiveEmitter(this);
                RuntimeManager.UpdateActiveEmitter(this, true);
            }
            else
            {
                PlayInstance();
            }
        }

        public void PlayInstance()
        {
            if (!instance.isValid())
            {
                instance.clearHandle();
            }

            // Let previous oneshot instances play out
            if (isOneshot && instance.isValid())
            {
                instance.release();
                instance.clearHandle();
            }

            bool is3D;
            eventDescription.is3D(out is3D);

            if (!instance.isValid())
            {
                eventDescription.createInstance(out instance);

                // Only want to update if we need to set 3D attributes
                if (is3D)
                {
                    var transform = GetComponent<Transform>();
#if UNITY_PHYSICS_EXIST
                    if (GetComponent<Rigidbody>())
                    {
                        Rigidbody rigidBody = GetComponent<Rigidbody>();
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject, rigidBody));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody);
                    }
                    else
#endif
#if UNITY_PHYSICS2D_EXIST
                    if (GetComponent<Rigidbody2D>())
                    {
                        var rigidBody2D = GetComponent<Rigidbody2D>();
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject, rigidBody2D));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody2D);
                    }
                    else
#endif
                    {
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform);
                    }
                }
            }

            foreach (var param in Params)
            {
                instance.setParameterByID(param.ID, param.Value);
            }

            foreach (var cachedParam in cachedParams)
            {
                instance.setParameterByID(cachedParam.ID, cachedParam.Value);
            }

            if (is3D && OverrideAttenuation)
            {
                instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, OverrideMinDistance);
                instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, OverrideMaxDistance);
            }

            instance.start();

            hasTriggered = true;
        }

        public void Stop()
        {
            RuntimeManager.DeregisterActiveEmitter(this);
            IsActive = false;
            cachedParams.Clear();
            StopInstance();
        }

        public void StopInstance()
        {
            if (TriggerOnce && hasTriggered)
            {
                RuntimeManager.DeregisterActiveEmitter(this);
            }

            if (instance.isValid())
            {
                instance.stop(AllowFadeout ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
                instance.release();
                instance.clearHandle();
            }
        }

        public void SetParameter(string name, float value, bool ignoreseekspeed = false)
        {
            if (Settings.Instance.StopEventsOutsideMaxDistance && IsActive)
            {
                ParamRef cachedParam = cachedParams.Find(x => x.Name == name);

                if (cachedParam == null)
                {
                    FMOD.Studio.PARAMETER_DESCRIPTION paramDesc;
                    eventDescription.getParameterDescriptionByName(name, out paramDesc);

                    cachedParam = new ParamRef();
                    cachedParam.ID = paramDesc.id;
                    cachedParam.Name = paramDesc.name;
                    cachedParams.Add(cachedParam);
                }

                cachedParam.Value = value;
            }

            if (instance.isValid())
            {
                instance.setParameterByName(name, value, ignoreseekspeed);
            }
        }

        public void SetParameter(FMOD.Studio.PARAMETER_ID id, float value, bool ignoreseekspeed = false)
        {
            if (Settings.Instance.StopEventsOutsideMaxDistance && IsActive)
            {
                ParamRef cachedParam = cachedParams.Find(x => x.ID.Equals(id));

                if (cachedParam == null)
                {
                    FMOD.Studio.PARAMETER_DESCRIPTION paramDesc;
                    eventDescription.getParameterDescriptionByID(id, out paramDesc);

                    cachedParam = new ParamRef();
                    cachedParam.ID = paramDesc.id;
                    cachedParam.Name = paramDesc.name;
                    cachedParams.Add(cachedParam);
                }

                cachedParam.Value = value;
            }

            if (instance.isValid())
            {
                instance.setParameterByID(id, value, ignoreseekspeed);
            }
        }

        public bool IsPlaying()
        {
            if (instance.isValid())
            {
                FMOD.Studio.PLAYBACK_STATE playbackState;
                instance.getPlaybackState(out playbackState);
                return (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED);
            }
            return false;
        }

        #region Autoculling
        void Update()
        {
            if (instance.isValid())
            {
                if (!occlusionEnabled)
                {
                    currentOcclusion_Final = 0.0f;
                }
                else if (Time.time >= nextOcclusionUpdate)
                {
                    nextOcclusionUpdate = Time.time + occlusionDetectionInterval;
                    currentOcclusion_Target = occlusionIntensity * ComputeOcclusion(transform);
                }

                //When target is different than actual state, update//
                if (currentOcclusion_Final != currentOcclusion_Target)
                {
                    if ((currentOcclusion_Final < 0 && currentOcclusion_Final >= currentOcclusion_Target - 0.1f) || (currentOcclusion_Final > 0 && currentOcclusion_Final <= currentOcclusion_Target + 0.1f))
                    {
                        currentOcclusion_Final = currentOcclusion_Target;
                    }
                    else
                    {
                        currentOcclusion_Final = Mathf.SmoothDamp(currentOcclusion_Final, currentOcclusion_Target, ref currentOcclusion_Calculation, 0.04f);
                    }

                    instance.setParameterByName(occlusionParameterName, currentOcclusion_Final);
                }
            }
        }

        static Transform ListenerTransform
        {
            get
            {
                if (listenerTransform == null)
                {
                    var listener = GameObject.FindObjectOfType<StudioListener>();
                    if (listener != null)
                    {
                        listenerTransform = listener.transform;
                    }
                }
                return listenerTransform;
            }
        }
        private static Transform listenerTransform = null;

        static float ComputeOcclusion(Transform sourceTransform)
        {
            var listener = GameObject.FindObjectOfType<StudioListener>();
            occlusionMaskValue = listener.occlusionMask;
            float occlusion = 0.0f;
            if (ListenerTransform != null)
            {
                Vector3 listenerPosition = ListenerTransform.position;
                Vector3 sourceFromListener = sourceTransform.position - listenerPosition;
                int numHits = Physics.RaycastNonAlloc(listenerPosition, sourceFromListener, occlusionHits, sourceFromListener.magnitude, occlusionMaskValue);
                for (int i = 0; i < numHits; ++i)
                {
                    if (occlusionHits[i].transform != listenerTransform && occlusionHits[i].transform != sourceTransform)
                    {
                        occlusion += 1.0f;
                    }
                }
            }
            return occlusion;
        }

        /// Maximum allowed number of raycast hits for occlusion computation per source.
        const int maxNumOcclusionHits = 12;

        // Pre-allocated raycast hit list for occlusion computation.
        private static RaycastHit[] occlusionHits = new RaycastHit[maxNumOcclusionHits];

        // Occlusion layer mask.
        private static int occlusionMaskValue = -1;

        /// Source occlusion detection rate in seconds.
        const float occlusionDetectionInterval = 0.1f;
        #endregion
    }
}


