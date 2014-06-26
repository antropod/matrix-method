using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Antropod.MatrixMethod
{
    [Serializable]
    public struct ApplicationData: INotifyPropertyChanged
    {
        private Final _final;
        public Stat _stat;

        public Stat stat
        {
            get { return _stat; }
            set
            {
                _stat = value;
                NotifyChanged("stat");
            }
        }

        public Final final
        {
            get { return _final; }
            set
            {
                _final = value;
                if (final == null)
                    stat = null;
                else
                    stat = new Stat(_final.maket.ComponentNames());
                NotifyChanged("final");
            }
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public static ApplicationData CreateDefault()
        {
            var result = new ApplicationData();
            result.final = new Final(Maket.CreateDefault());
            return result;
        }

        public static ApplicationData LoadFromFile(string filename)
        {
            try
            {
                return (ApplicationData)Utility.Deserialize(filename);
            }
            catch (Exception)
            {
                return CreateDefault();
            }
        }
    }
}
