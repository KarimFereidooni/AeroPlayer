using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AeroPlayer
{
    public class PlayListManager : IEnumerable<Media>
    {
        public PlayListManager()
        {
            this.PlayList = new List<Media>();
        }

        public void Load()
        {
            ClearPlayList();
            foreach (var item in Properties.Settings.Default.PlayList)
            {
                AddToPlayList(new Media(item));
            }
        }

        public void Save()
        {
            Properties.Settings.Default.PlayList.Clear();
            foreach (var item in this)
            {
                Properties.Settings.Default.PlayList.Add(item.FilePath);
            }
            Properties.Settings.Default.Save();
        }

        public void SetPlayList(List<Media> PlayList)
        {
            this.PlayList = PlayList;
        }
        public int AddToPlayList(Media file)
        {
            if (PlayList.Contains(file))
                return PlayList.IndexOf(file);
            else
            {
                this.PlayList.Add(file);
                return PlayList.Count - 1;
            }
        }
        public int IndexOf(Media item)
        {
            return PlayList.IndexOf(item);
        }
        public void ClearPlayList()
        {
            PlayList.Clear();
        }
        public int Count
        {
            get
            {
                return PlayList.Count;
            }
        }
        public Media this[int index]
        {
            get
            {
                return PlayList[index];
            }
        }
        public Media this[string FilePath]
        {
            get
            {
                foreach (var item in this)
                {
                    if (item.FilePath.ToLower() == FilePath.ToLower())
                        return item;
                }
                return null;
            }
        }
        private List<Media> _PlayList;
        private List<Media> PlayList
        {
            get
            {
                return _PlayList;
            }
            set
            {
                _PlayList = value;
            }
        }

        #region IEnumerable<string> Members

        IEnumerator<Media> IEnumerable<Media>.GetEnumerator()
        {
            return this.PlayList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.PlayList.GetEnumerator();
        }

        #endregion
    }
}
