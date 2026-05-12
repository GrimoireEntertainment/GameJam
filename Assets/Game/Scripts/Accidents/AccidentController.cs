using System;
using System.Collections.Generic;
using Game.Audio;
using Game.Core;
using Game.Level;
using UnityEngine;

namespace Game.Accidents
{
    public class AccidentController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Ship health controller damaged by active accidents.")]
        private ShipHealthController _shipHealthController;

        [Header("Schedule")]
        [SerializeField, Tooltip("Accidents triggered by elapsed level time.")]
        private List<LevelAccidentScheduleEntry> _schedule = new();

        [Header("Audio")]
        [SerializeField, Tooltip("Accident Id that plays the fire loop.")]
        private string _fireAccidentId = "Fire";

        [SerializeField, Tooltip("Accident Id that plays the hull breach impact.")]
        private string _hullBreachAccidentId = "HullBreach";

        private readonly Dictionary<string, ActiveAccident> _activeAccidents = new();
        private readonly List<IAccidentLocation> _accidentLocations = new();
        private readonly List<AccidentLinkedObject> _linkedObjects = new();
        private readonly Dictionary<string, AudioSource> _alarmSourcesByAccidentId = new();
        private readonly HashSet<int> _triggeredScheduleEntries = new();
        private float _elapsedTime;
        private int _nextInstanceNumber;
        private int _activeFireAccidentCount;

        public IReadOnlyCollection<ActiveAccident> ActiveAccidents => _activeAccidents.Values;

        public event Action<ActiveAccident> AccidentStarted;
        public event Action<ActiveAccident> AccidentResolved;

        private void OnDisable()
        {
            StopAllAccidentAudio();
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            UpdateSchedule();
        }

        public ActiveAccident StartAccident(AccidentDefinition accident)
        {
            return StartAccident(accident, string.Empty);
        }

        public ActiveAccident StartAccident(AccidentDefinition accident, string locationId)
        {
            if (accident == null || string.IsNullOrWhiteSpace(accident.Id))
            {
                return null;
            }

            IAccidentLocation accidentLocation = FindAvailableAccidentLocation(accident.Id, locationId);
            AccidentLinkedObject linkedObject = accidentLocation == null ? FindAvailableLinkedObject(accident.Id, locationId) : null;

            if (accidentLocation == null && linkedObject == null && HasLocationForType(accident.Id))
            {
                return null;
            }

            string resolvedLocationId = accidentLocation != null ? accidentLocation.LocationId : linkedObject != null ? linkedObject.LocationId : locationId;
            string instanceId = CreateInstanceId(accident.Id, resolvedLocationId);
            ActiveAccident activeAccident = new(instanceId, accident, resolvedLocationId);

            _activeAccidents.Add(instanceId, activeAccident);
            _shipHealthController?.AddDamageSource(instanceId, accident.DamagePerSecond);
            accidentLocation?.Activate(activeAccident);
            linkedObject?.Activate(activeAccident);
            PlayAccidentStartedAudio(activeAccident);
            AccidentStarted?.Invoke(activeAccident);
            return activeAccident;
        }

        public void ResolveAccident(string accidentId)
        {
            if (string.IsNullOrWhiteSpace(accidentId))
            {
                return;
            }

            if (!_activeAccidents.TryGetValue(accidentId, out ActiveAccident accident))
            {
                accident = FindFirstActiveAccidentByType(accidentId);

                if (accident == null)
                {
                    return;
                }
            }

            _activeAccidents.Remove(accident.InstanceId);
            _shipHealthController?.RemoveDamageSource(accident.InstanceId);
            FindAccidentLocation(accident)?.Deactivate(accident);
            FindLinkedObject(accident)?.Deactivate(accident);
            PlayAccidentResolvedAudio(accident);
            AccidentResolved?.Invoke(accident);
        }

        public bool IsAccidentActive(string accidentId)
        {
            return !string.IsNullOrWhiteSpace(accidentId)
                && (_activeAccidents.ContainsKey(accidentId) || FindFirstActiveAccidentByType(accidentId) != null);
        }

        public void RegisterLinkedObject(AccidentLinkedObject linkedObject)
        {
            if (linkedObject == null || _linkedObjects.Contains(linkedObject))
            {
                return;
            }

            _linkedObjects.Add(linkedObject);
        }

        public void UnregisterLinkedObject(AccidentLinkedObject linkedObject)
        {
            if (linkedObject == null)
            {
                return;
            }

            _linkedObjects.Remove(linkedObject);
        }

        public void RegisterRepairPoint(HullBreachRepairPoint repairPoint)
        {
            RegisterAccidentLocation(repairPoint);
        }

        public void UnregisterRepairPoint(HullBreachRepairPoint repairPoint)
        {
            UnregisterAccidentLocation(repairPoint);
        }

        public void RegisterAccidentLocation(IAccidentLocation accidentLocation)
        {
            if (accidentLocation == null || _accidentLocations.Contains(accidentLocation))
            {
                return;
            }

            _accidentLocations.Add(accidentLocation);
        }

        public void UnregisterAccidentLocation(IAccidentLocation accidentLocation)
        {
            if (accidentLocation == null)
            {
                return;
            }

            _accidentLocations.Remove(accidentLocation);
        }

        private void UpdateSchedule()
        {
            for (int i = 0; i < _schedule.Count; i++)
            {
                if (_triggeredScheduleEntries.Contains(i))
                {
                    continue;
                }

                LevelAccidentScheduleEntry entry = _schedule[i];

                if (entry == null || entry.Accident == null)
                {
                    _triggeredScheduleEntries.Add(i);
                    continue;
                }

                if (_elapsedTime >= entry.TriggerTime)
                {
                    _triggeredScheduleEntries.Add(i);
                    StartAccident(entry.Accident, entry.LocationId);
                }
            }
        }

        private AccidentLinkedObject FindAvailableLinkedObject(string accidentTypeId, string locationId)
        {
            for (int i = 0; i < _linkedObjects.Count; i++)
            {
                AccidentLinkedObject linkedObject = _linkedObjects[i];

                if (linkedObject == null || !linkedObject.Matches(accidentTypeId, locationId) || linkedObject.IsActive)
                {
                    continue;
                }

                return linkedObject;
            }

            return null;
        }

        private IAccidentLocation FindAvailableAccidentLocation(string accidentTypeId, string locationId)
        {
            for (int i = 0; i < _accidentLocations.Count; i++)
            {
                IAccidentLocation accidentLocation = _accidentLocations[i];

                if (accidentLocation == null || !accidentLocation.Matches(accidentTypeId, locationId) || accidentLocation.IsActive)
                {
                    continue;
                }

                return accidentLocation;
            }

            return null;
        }

        private IAccidentLocation FindAccidentLocation(ActiveAccident accident)
        {
            for (int i = 0; i < _accidentLocations.Count; i++)
            {
                IAccidentLocation accidentLocation = _accidentLocations[i];

                if (accidentLocation != null && accidentLocation.ActiveInstanceId == accident.InstanceId)
                {
                    return accidentLocation;
                }
            }

            return null;
        }

        private AccidentLinkedObject FindLinkedObject(ActiveAccident accident)
        {
            for (int i = 0; i < _linkedObjects.Count; i++)
            {
                AccidentLinkedObject linkedObject = _linkedObjects[i];

                if (linkedObject != null && linkedObject.ActiveInstanceId == accident.InstanceId)
                {
                    return linkedObject;
                }
            }

            return null;
        }

        private bool HasLocationForType(string accidentTypeId)
        {
            return HasAccidentLocationForType(accidentTypeId) || HasLinkedObjectForType(accidentTypeId);
        }

        private bool HasAccidentLocationForType(string accidentTypeId)
        {
            for (int i = 0; i < _accidentLocations.Count; i++)
            {
                IAccidentLocation accidentLocation = _accidentLocations[i];

                if (accidentLocation != null && accidentLocation.AccidentTypeId == accidentTypeId)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasLinkedObjectForType(string accidentTypeId)
        {
            for (int i = 0; i < _linkedObjects.Count; i++)
            {
                AccidentLinkedObject linkedObject = _linkedObjects[i];

                if (linkedObject != null && linkedObject.AccidentTypeId == accidentTypeId)
                {
                    return true;
                }
            }

            return false;
        }

        private ActiveAccident FindFirstActiveAccidentByType(string accidentTypeId)
        {
            foreach (ActiveAccident accident in _activeAccidents.Values)
            {
                if (accident.TypeId == accidentTypeId)
                {
                    return accident;
                }
            }

            return null;
        }

        private void PlayAccidentStartedAudio(ActiveAccident accident)
        {
            if (accident == null)
            {
                return;
            }

            if (accident.TypeId == _fireAccidentId)
            {
                if (_activeFireAccidentCount == 0)
                {
                    AudioService.Instance?.StartLoop(GameSoundId.FireBurning);
                }

                _activeFireAccidentCount++;
            }

            if (accident.TypeId == _hullBreachAccidentId)
            {
                AudioService.Instance?.PlaySfx(GameSoundId.HullBreachImpact);
            }

            AudioSource alarmSource = AudioService.Instance?.StartLoopInstance(GameSoundId.Alarm);

            if (alarmSource != null)
            {
                _alarmSourcesByAccidentId[accident.InstanceId] = alarmSource;
            }
        }

        private void PlayAccidentResolvedAudio(ActiveAccident accident)
        {
            if (accident == null)
            {
                return;
            }

            if (_alarmSourcesByAccidentId.TryGetValue(accident.InstanceId, out AudioSource alarmSource))
            {
                _alarmSourcesByAccidentId.Remove(accident.InstanceId);
                AudioService.Instance?.StopLoopInstance(alarmSource);
            }

            if (accident.TypeId == _fireAccidentId)
            {
                _activeFireAccidentCount = Mathf.Max(0, _activeFireAccidentCount - 1);

                if (_activeFireAccidentCount == 0)
                {
                    AudioService.Instance?.StopLoop(GameSoundId.FireBurning);
                }
            }
        }

        private void StopAllAccidentAudio()
        {
            foreach (AudioSource alarmSource in _alarmSourcesByAccidentId.Values)
            {
                AudioService.Instance?.StopLoopInstance(alarmSource);
            }

            _alarmSourcesByAccidentId.Clear();

            if (_activeFireAccidentCount > 0)
            {
                _activeFireAccidentCount = 0;
                AudioService.Instance?.StopLoop(GameSoundId.FireBurning);
            }
        }

        private string CreateInstanceId(string accidentTypeId, string locationId)
        {
            _nextInstanceNumber++;

            if (string.IsNullOrWhiteSpace(locationId))
            {
                return $"{accidentTypeId}_{_nextInstanceNumber}";
            }

            return $"{accidentTypeId}_{locationId}_{_nextInstanceNumber}";
        }
    }
}
