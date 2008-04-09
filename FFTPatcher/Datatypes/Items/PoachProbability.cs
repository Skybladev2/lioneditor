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
    /// Represent's the common and uncommon items that can be poached from a monster.
    /// </summary>
    public class PoachProbability : IChangeable
    {

		#region Properties (5) 


        public Item Common { get; set; }

        public PoachProbability Default { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has changed.
        /// </summary>
        /// <value></value>
        public bool HasChanged
        {
            get { return 
                Default != null && 
                (Common.Offset != Default.Common.Offset || 
                Uncommon.Offset != Default.Uncommon.Offset); }
        }

        public string MonsterName { get; private set; }

        public Item Uncommon { get; set; }


		#endregion Properties 

		#region Constructors (2) 

        public PoachProbability( string name, IList<byte> bytes )
            : this( name, bytes, null )
        {
        }

        public PoachProbability( string name, IList<byte> bytes, PoachProbability defaults )
        {
            Default = defaults;
            MonsterName = name;
            Common = Item.GetItemAtOffset( bytes[0] );
            Uncommon = Item.GetItemAtOffset( bytes[1] );
        }

		#endregion Constructors 

		#region Methods (1) 


        public byte[] ToByteArray()
        {
            byte[] result = new byte[2];
            result[0] = (byte)(Common.Offset & 0xFF);
            result[1] = (byte)(Uncommon.Offset & 0xFF);
            return result;
        }


		#endregion Methods 

    }

    public class AllPoachProbabilities : IChangeable
    {

		#region Properties (2) 


        /// <summary>
        /// Gets a value indicating whether this instance has changed.
        /// </summary>
        /// <value></value>
        public bool HasChanged
        {
            get 
            {
                foreach( PoachProbability p in PoachProbabilities )
                {
                    if( p.HasChanged )
                        return true;
                }

                return false;
            }
        }

        public PoachProbability[] PoachProbabilities { get; private set; }


		#endregion Properties 

		#region Constructors (1) 

        public AllPoachProbabilities( IList<byte> bytes )
        {
            byte[] defaultBytes = FFTPatch.Context == Context.US_PSP ? Resources.PoachProbabilitiesBin : PSXResources.PoachProbabilitiesBin;

            PoachProbabilities = new PoachProbability[48];
            for( int i = 0; i < 48; i++ )
            {
                PoachProbabilities[i] = new PoachProbability( AllJobs.Names[i + 0x5E], bytes.Sub( i * 2, i * 2 + 1 ),
                    new PoachProbability( AllJobs.Names[i + 0x5E], defaultBytes.Sub( i * 2, i * 2 + 1 ) ) );
            }
        }

		#endregion Constructors 

		#region Methods (3) 


        public List<string> GenerateCodes()
        {
            if( FFTPatch.Context == Context.US_PSP )
            {
                return Codes.GenerateCodes( Context.US_PSP, Resources.PoachProbabilitiesBin, this.ToByteArray(), 0x27AFD0 );
            }
            else
            {
                return Codes.GenerateCodes( Context.US_PSX, PSXResources.PoachProbabilitiesBin, this.ToByteArray(), 0x066064 );
            }
        }

        public byte[] ToByteArray()
        {
            List<byte> result = new List<byte>( 96 );
            foreach( PoachProbability p in PoachProbabilities )
            {
                result.AddRange( p.ToByteArray() );
            }

            return result.ToArray();
        }

        public byte[] ToByteArray( Context context )
        {
            return ToByteArray();
        }


		#endregion Methods 

    }
}
