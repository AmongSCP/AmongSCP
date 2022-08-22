using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmongSCP.PlayerManager
{
    public class PlayerInfo
    {
        private readonly PlayerManager _manager;

        private Role _role = Role.None;
        public Role Role
        {
            get => _role;
            set
            {
                _role = value;

                _manager.ReloadLists();
            }
        }

        private bool _isAlive = false;
        public bool IsAlive 
        {
            get => _isAlive;
            set
            {
                _isAlive = value;

                _manager.ReloadLists();
            }
        }

        public Vector3 DeadPos = Vector3.zero;

        public bool CalledEmergencyMeeting = false;

        public DateTime LastShot = DateTime.MinValue;

        public DateTime LastVent = DateTime.MinValue;

        public int EmergencyMeetings = AmongSCP.Singleton.Config.EmergencyMeetings;

        public int votes = 0;

        public bool hasVoted = false;

        public bool skipped = false;

        public List<object> CompletedTasks = new List<object>();
        




        public PlayerInfo(PlayerManager manager)
        {
            _manager = manager;

            _manager.ReloadLists();
        }
    }
}