using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillDamage
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        public static T mInstance;

        public static T getInstance()
        {
            if (mInstance == null)
            {
                mInstance = new T();
            }
            return mInstance;
        }

        public virtual void destroy()
        {
        }

        public static void destroyInstance()
        {
            if (mInstance != null)
            {
                mInstance.destroy();
                mInstance = (T)null;
            }
        }
    }
}
