﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FFTPatcher.SpriteEditor
{
    public partial class FullSpriteSetEditor : UserControl
    {
        public FullSpriteSet FullSpriteSet { get; private set; }

        private class FlickerFreeListView : ListView
        {
            public FlickerFreeListView()
                : base()
            {
                DoubleBuffered = true;
            }
        }

        public FullSpriteSetEditor()
        {
            InitializeComponent();
            listView1.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler( listView1_RetrieveVirtualItem );
            listView1.CacheVirtualItems += new CacheVirtualItemsEventHandler( listView1_CacheVirtualItems );
            listView1.Enabled = false;
            listView1.Activation = ItemActivation.Standard;
            listView1.ItemActivate += new EventHandler( listView1_ItemActivate );
        }

        public class ImageEventArgs : EventArgs
        {
            public AbstractSprite Sprite { get; private set; }
            public ImageEventArgs( AbstractSprite sprite )
            {
                Sprite = sprite;
            }
        }

        public event EventHandler<ImageEventArgs> ImageActivated;

        void listView1_ItemActivate( object sender, EventArgs e )
        {
            if ( ImageActivated != null )
            {
                ImageActivated( this, new ImageEventArgs( FullSpriteSet.Sprites[listView1.SelectedIndices[0]] ) );
            }
            
        }

        void listView1_CacheVirtualItems( object sender, CacheVirtualItemsEventArgs e )
        {
        }

        private List<ListViewItem> items;

        /// <summary>
        /// Loads the full sprite set.
        /// </summary>
        /// <param name="set">The set.</param>
        public void LoadFullSpriteSet( FullSpriteSet set )
        {
            items = new List<ListViewItem>();

            listView1.LargeImageList = set.Thumbnails;

            IList<AbstractSprite> sprites = set.Sprites;
            for( int i = 0; i < sprites.Count; i++ )
            {
                items.Add( new ListViewItem( sprites[i].Name, i ) );
            }

            listView1.Enabled = true;
            listView1.VirtualListSize = set.Sprites.Count;
            
            FullSpriteSet = set;

        }

        private void listView1_RetrieveVirtualItem( object sender, RetrieveVirtualItemEventArgs e )
        {
            if( items != null && e.ItemIndex < items.Count )
            {
                e.Item = items[e.ItemIndex];
            }
        }
    }
}
