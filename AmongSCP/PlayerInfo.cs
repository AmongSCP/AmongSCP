using System;
using UnityEngine;

namespace AmongSCP
{
    public class PlayerInfo
    {
        private readonly PlayerManager _manager;

        private Role _role = Role.NONE;
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

        public DateTime LastShot = DateTime.Now.Subtract(TimeSpan.FromSeconds(30));

        public PlayerInfo(PlayerManager manager)
        {
            _manager = manager;
            
            _manager.ReloadLists();
        }
    }
}