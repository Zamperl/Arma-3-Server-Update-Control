using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BatchWriter
{
    public class AddonItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                    _writeSettings?.Invoke();
                }
            }
        }

        private int _updateStatus;
        /// <summary>
        /// 0: unkown, 1: finished, 2: redo, 3: failed
        /// </summary>
        public int UpdateStatus
        {
            get { return _updateStatus; }
            set
            {
                if (value != _updateStatus)
                {
                    _updateStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private Action _writeSettings;

        public string ID { get; set; }
        public string Name { get; set; }

        public AddonItem(Action writeSettings)
        {
            _writeSettings = writeSettings;
        }

        #region Notifypropertychanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
