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
using System.Xml.Serialization;

namespace FFTPatcher.TextEditor.Files
{
    public interface IStringSectioned : IXmlSerializable, IFile
    {
        string Filename { get; }
        IList<IList<string>> Sections { get; }
        IList<string> SectionNames { get; }
        IList<IList<string>> EntryNames { get; }
        int EstimatedLength { get; }
        int ActualLength { get; }
        int MaxLength { get; }
    }
}
