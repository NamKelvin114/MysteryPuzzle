namespace Worldreaver.Utility
{
    /// <summary>
    /// flag bit operation
    /// author: @kazupon
    /// </summary>
    public class BitFlag<T> where T : struct
    {
        private int _flag;

        /// <summary>
        /// add flag
        /// </summary>
        /// <param name="flag"></param>
        public void AddFlag(T flag)
        {
            this._flag |= 1 << GetIntValue(flag);
        }

        /// <summary>
        /// remove flag
        /// </summary>
        /// <param name="flag"></param>
        public void RemoveFlag(T flag)
        {
            this._flag &= ~(1 << GetIntValue(flag));
        }

        /// <summary>
        /// set flag
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="b"></param>
        public void SetFlag(T flag, bool b)
        {
            if (b)
            {
                AddFlag(flag);
            }
            else
            {
                RemoveFlag(flag);
            }
        }

        /// <summary>
        /// check flag
        /// </summary>
        /// <param name="flag"></param>
        public bool HasFlag(T flag)
        {
            return (this._flag & (1 << GetIntValue(flag))) > 0;
        }

        /// <summary>
        /// Check flag (only once, break after checking)
        /// </summary>
        /// <param name="flag"></param>
        public bool OneTimeFlag(T flag)
        {
            var temp = HasFlag(flag);
            RemoveFlag(flag);
            return temp;
        }

        /// <summary>
        /// reset flag
        /// </summary>
        public void ResetFlag()
        {
            _flag = 0;
        }

        /// <summary>
        /// convert to int
        /// </summary>
        private int GetIntValue(T flag)
        {
            return (int) ((object) flag);
        }
    }

    /// <summary>
    /// BitArray Extension
    /// author: @kazupon
    /// </summary>
    public static class BitArrayExtensions
    {
        /// <summary>
        /// check if any bit is ON.
        /// </summary>
        public static bool Any(this System.Collections.BitArray self)
        {
            foreach (bool bit in self)
            {
                if (bit)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// check if all bit is ON.
        /// </summary>
        public static bool All(this System.Collections.BitArray self)
        {
            foreach (bool bit in self)
            {
                if (!bit)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// gets whether all bits are OFF.
        /// </summary>
        public static bool None(this System.Collections.BitArray self)
        {
            return !Any(self);
        }

        /// <summary>
        ///Invert the specified bit.
        /// </summary>
        public static void Flip(this System.Collections.BitArray self, int index)
        {
            self.Set(index, !self.Get(index));
        }
    }
}