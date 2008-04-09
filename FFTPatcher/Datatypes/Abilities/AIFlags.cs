﻿/*
    Copyright 2007, Joe Davidson <joedavidson@gmail.com>

    This file is part of FFTPatcher.

    FFTPatcher is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    FFTPatcher is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with FFTPatcher.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;

namespace FFTPatcher.Datatypes
{
    /// <summary>
    /// Represents an <see cref="Ability"/>'s AI behavior.
    /// </summary>
    public class AIFlags
    {

		#region Fields (24) 

        public bool AddStatus;
        public bool Blank;
        public bool CancelStatus;
        public bool DefenseUp;
        public bool DirectAttack;
        public bool HP;
        public bool IgnoreRange;
        public bool LineAttack;
        public bool MagicDefenseUp;
        public bool MP;
        public bool RandomHits;
        public bool Reflectable;
        public bool Silence;
        public bool Stats;
        public bool TargetAllies;
        public bool TargetEnemies;
        public bool TripleAttack;
        public bool TripleBracelet;
        public bool UndeadReverse;
        public bool Unequip;
        public bool Unknown1;
        public bool Unknown2;
        public bool Unknown3;
        public bool VerticalIncrease;

		#endregion Fields 

		#region Constructors (1) 

        public AIFlags( IList<byte> bytes )
        {
            Utilities.CopyByteToBooleans( bytes[0],
                ref HP, ref MP, ref CancelStatus, ref AddStatus, ref Stats, ref Unequip, ref TargetEnemies, ref TargetAllies );

            Utilities.CopyByteToBooleans( bytes[1],
                ref IgnoreRange, ref Reflectable, ref UndeadReverse, ref Unknown1, ref RandomHits, ref Unknown2, ref Unknown3, ref Silence );
            Silence = !Silence;

            Utilities.CopyByteToBooleans( bytes[2],
                ref Blank, ref DirectAttack, ref LineAttack, ref VerticalIncrease, ref TripleAttack, ref TripleBracelet, ref MagicDefenseUp, ref DefenseUp );
            VerticalIncrease = !VerticalIncrease;
        }

		#endregion Constructors 

		#region Methods (2) 


        public bool[] ToBoolArray()
        {
            return new bool[24] { 
                HP, MP, CancelStatus, AddStatus, Stats, Unequip, TargetEnemies, TargetAllies,
                IgnoreRange, Reflectable, UndeadReverse, Unknown1, RandomHits, Unknown2, Unknown3, Silence,
                Blank, DirectAttack, LineAttack, VerticalIncrease, TripleAttack, TripleBracelet, MagicDefenseUp, DefenseUp };
        }

        public byte[] ToByteArray()
        {
            byte[] result = new byte[3];
            result[0] = Utilities.ByteFromBooleans( HP, MP, CancelStatus, AddStatus, Stats, Unequip, TargetEnemies, TargetAllies );
            result[1] = Utilities.ByteFromBooleans( IgnoreRange, Reflectable, UndeadReverse, Unknown1, RandomHits, Unknown2, Unknown3, !Silence );
            result[2] = Utilities.ByteFromBooleans( Blank, DirectAttack, LineAttack, !VerticalIncrease, TripleAttack, TripleBracelet, MagicDefenseUp, DefenseUp );
            return result;
        }


		#endregion Methods 

    }
}
