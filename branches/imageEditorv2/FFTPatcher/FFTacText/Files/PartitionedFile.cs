﻿using System;
using System.Collections.Generic;
using PatcherLib.Datatypes;
using PatcherLib.Utilities;
using System.Text;

namespace FFTPatcher.TextEditor
{
    class PartitionedFile : AbstractFile
    {
        public int PartitionSize { get; private set; }

        public PartitionedFile( GenericCharMap map, FFTTextFactory.FileInfo layout, IList<IList<string>> strings )
            : base( map, layout, strings, false )
        {
            PartitionSize = layout.Size / NumberOfSections;
        }

        public PartitionedFile( GenericCharMap map, FFTPatcher.TextEditor.FFTTextFactory.FileInfo layout, IList<byte> bytes )
            : base( map, layout, false )
        {
            PartitionSize = layout.Size / NumberOfSections;
            List<IList<string>> sections = new List<IList<string>>( NumberOfSections );
            for ( int i = 0; i < NumberOfSections; i++ )
            {
                int terminatorCount = layout.AllowedTerminators.Count;
                IList<string>[] stringsFromEachTerminator = new IList<string>[terminatorCount];
                for ( int t = 0; t < terminatorCount; t++ )
                {
                    stringsFromEachTerminator[t] =
                        TextUtilities.ProcessList( 
                            bytes.Sub( i * PartitionSize, ( i + 1 ) * PartitionSize - 1 ), 
                            layout.AllowedTerminators[t], 
                            map );
                }

                // Determine best fit terminator
                IList<string> best = stringsFromEachTerminator[0];
                for ( int t = 1; t < terminatorCount; t++ )
                {
                    if ( ( best.Count < SectionLengths[i] && stringsFromEachTerminator[t].Count < SectionLengths[i] &&
                           ( SectionLengths[i] - stringsFromEachTerminator[t].Count ) < ( SectionLengths[i] - best.Count ) ) ||
                         ( best.Count != SectionLengths[i] && stringsFromEachTerminator[t].Count == SectionLengths[i] ) ||
                         ( best.Count > SectionLengths[i] && stringsFromEachTerminator[t].Count > SectionLengths[i] &&
                           ( stringsFromEachTerminator[t].Count - SectionLengths[i] ) < ( best.Count - SectionLengths[i] ) )
                        )
                    {
                        best = stringsFromEachTerminator[t];
                    }
                }

                sections.Add( best );

                if ( sections[i].Count < SectionLengths[i] )
                {
                    string[] newSection = new string[SectionLengths[i]];
                    sections[i].CopyTo( newSection, 0 );
                    new string[SectionLengths[i] - sections[i].Count].CopyTo( newSection, sections[i].Count );
                    sections[i] = newSection;
                }
                else if (sections[i].Count > SectionLengths[i])
                {
                    sections[i] = sections[i].Sub(0, SectionLengths[i] - 1);
                }

                System.Diagnostics.Debug.Assert(sections[i].Count == SectionLengths[i]);
            }
            Sections = sections.AsReadOnly();
            PopulateDisallowedSections();
        }

        private Set<KeyValuePair<string, byte>> GetPreferredDTEPairsForSection(IList<IList<string>> allSections, int index, Set<string> replacements, Set<KeyValuePair<string, byte>> currentPairs, Stack<byte> dteBytes)
        {
            var secs = allSections;
            var  bytes = GetSectionByteArrays(secs, SelectedTerminator, CharMap, CompressionAllowed);
            IList<byte> ourBytes = bytes[index];

            Set<KeyValuePair<string, byte>> result = new Set<KeyValuePair<string, byte>>();

            int bytesNeeded = ourBytes.Count - this.PartitionSize;
            if (bytesNeeded <= 0)
            {
                return result;
            }
            string terminatorString = string.Format( @"{0x" + "{0:2X}" + "}", SelectedTerminator );

            StringBuilder sb = new StringBuilder(PartitionSize);
            if (DteAllowed[index])
            {
                secs[index].ForEach(t => sb.Append(t).Append(terminatorString));
            }

            var dict = TextUtilities.GetPairAndTripleCounts(sb.ToString(), replacements);

            var l = new List<KeyValuePair<string, int>>(dict);
            l.Sort((a, b) => b.Value.CompareTo(a.Value));

            while (bytesNeeded > 0 && l.Count > 0 && dteBytes.Count > 0)
            {
                result.Add(new KeyValuePair<string, byte>(l[0].Key, dteBytes.Pop()));
                TextUtilities.DoDTEEncoding(secs, DteAllowed, PatcherLib.Utilities.Utilities.DictionaryFromKVPs(result));
                bytes = GetSectionByteArrays(secs, SelectedTerminator, CharMap, CompressionAllowed);
                ourBytes = bytes[index];
                bytesNeeded = ourBytes.Count - PartitionSize;

                if (bytesNeeded > 0)
                {
                    StringBuilder sb2 = new StringBuilder(PartitionSize);
                    if (DteAllowed[index])
                    {
                        secs[index].ForEach(t => sb2.Append(t).Append(terminatorString));
                    }
                    l = new List<KeyValuePair<string, int>>(TextUtilities.GetPairAndTripleCounts(sb2.ToString(), replacements));
                    l.Sort((a, b) => b.Value.CompareTo(a.Value));
                }
            }


            if (bytesNeeded > 0)
            {
                return null;
            }
            return result;
        }

        public override Set<KeyValuePair<string, byte>> GetPreferredDTEPairs(Set<string> replacements, Set<KeyValuePair<string, byte>> currentPairs, Stack<byte> dteBytes)
        {
            Set<KeyValuePair<string, byte>> result = new Set<KeyValuePair<string, byte>>();
            Set<KeyValuePair<string, byte>> ourCurrentPairs = new Set<KeyValuePair<string, byte>>(currentPairs);
            for (int i = 0; i < Sections.Count; i++)
            {
                result.AddRange(GetPreferredDTEPairsForSection(GetCopyOfSections(), i, replacements, ourCurrentPairs, dteBytes));
                ourCurrentPairs.AddRange(result);
            }
            return result;
        }

        protected override IList<byte> ToByteArray()
        {
            List<byte> result = new List<byte>( Layout.Size );
            foreach ( IList<string> section in Sections )
            {
                List<byte> currentPart = new List<byte>( PartitionSize );
                section.ForEach( s => currentPart.AddRange( CharMap.StringToByteArray( s, SelectedTerminator ) ) );
                currentPart.AddRange( new byte[Math.Max( PartitionSize - currentPart.Count, 0 )] );
                result.AddRange( currentPart );
            }

            return result.AsReadOnly();
        }

        protected override IList<byte> ToByteArray( IDictionary<string, byte> dteTable )
        {
            // Clone the sections
            var secs = GetCopyOfSections();
            TextUtilities.DoDTEEncoding(secs, DteAllowed, dteTable);
            List<byte> result = new List<byte>(Layout.Size);
            foreach (IList<string> section in secs)
            {
                List<byte> currentPart = new List<byte>(PartitionSize);
                section.ForEach(s => currentPart.AddRange(CharMap.StringToByteArray(s, SelectedTerminator)));
                if (currentPart.Count > PartitionSize)
                {
                    return null;
                }
                currentPart.AddRange(new byte[Math.Max(PartitionSize - currentPart.Count, 0)]);
                result.AddRange(currentPart);
            }

            if (result.Count > Layout.Size)
            {
                return null;
            }
            return result.AsReadOnly();
        }
    }
}
