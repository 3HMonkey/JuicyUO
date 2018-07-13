#region license
//  Copyright (C) 2018 JuicyUO Development Community on Github
//
//	This project is an alternative client for the game Ultima Online.
//	The goal of this is to develop a lightweight client considering 
//	new technologies such as DirectX (MonoGame included). The foundation
//	is originally licensed (GNU) on JuicyUO and the JuicyUO Development
//	Team. (Copyright (c) 2015 JuicyUO Development Team)
//    
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.
#endregion

#region usings
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace JuicyUO.Ultima.IO
{
    class MulFileIndex : AFileIndex
    {
        private readonly string IndexPath;
        public int patchFile { get; set; }

        /// <summary>
        /// Creates a reference to an index file. (Ex: anim.idx)
        /// </summary>
        /// <param name="idxFile">Name of .idx file in UO base directory.</param>
        /// <param name="mulFile">Name of .mul file that this index file provides an index for.</param>
        /// <param name="length">Number of indexes in this index file.</param>
        /// <param name="patch_file">Index to patch data in Versioning.</param>
        public MulFileIndex(string idxFile, string mulFile, int length, int patch_file)
            : base(mulFile)
        {
            IndexPath = FileManager.GetFilePath(idxFile);
            Length = length;
            patchFile = patch_file;  
            Open();
        }

        protected override FileIndexEntry3D[] ReadEntries()
        {
            if (!File.Exists(IndexPath) || !File.Exists(DataPath))
            {
                return new FileIndexEntry3D[0];
            }

            List<FileIndexEntry3D> entries = new List<FileIndexEntry3D>();

            int length = (int)((new FileInfo(IndexPath).Length / 3) / 4);

            using (FileStream index = new FileStream(IndexPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                BinaryReader bin = new BinaryReader(index);

                int count = (int)(index.Length / 12);

                for (int i = 0; i < count && i < length; ++i)
                {
                    FileIndexEntry3D entry = new FileIndexEntry3D(bin.ReadInt32(), bin.ReadInt32(), bin.ReadInt32());
                    entries.Add(entry);
                }

                for (int i = count; i < length; ++i)
                {
                    FileIndexEntry3D entry = new FileIndexEntry3D(-1, -1, -1);
                    entries.Add(entry);
                }
            }

            FileIndexEntry5D[] patches = VerData.Patches;

            for (int i = 0; i < patches.Length; ++i)
            {
                FileIndexEntry5D patch = patches[i];

                if (patch.file == patchFile && patch.index >= 0 && patch.index < entries.Count)
                {
                    FileIndexEntry3D entry = entries.ElementAt(patch.index);
                    entry.Lookup = patch.lookup;
                    entry.Length = patch.length | (1 << 31);
                    entry.Extra = patch.extra;
                }
            }

            return entries.ToArray();
        }

    }
}
