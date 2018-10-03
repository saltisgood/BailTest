using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly BailCond mCondList;
        private readonly List<Panel> mEntries;

        public Form1()
        {
            InitializeComponent();

            mCondList = BailCond.CreateDefault();

            mEntries = new List<Panel>();
            mEntries.Add(mNamePanel);
            mEntries.Add(mAddressPanel);
            mEntries.Add(mOtherPanel);

            foreach (var entry in mEntries)
                entry.Visible = false;

            //PopulateList();
            listBox1.Items.Add("Name");
            listBox1.Items.Add("Address");
            listBox1.Items.Add("Other");
        }

        private void PopulateList()
        {
            int i = 1;
            foreach (BailCond.Condition cond in mCondList.Conditions)
            {
                listBox1.Items.Add(String.Format("Condition {0}: {1}", i, cond.ShortText));
                ++i;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var entry in mEntries)
                entry.Visible = false;

            foreach (int idx in listBox1.SelectedIndices)
            {
                mEntries[idx].Visible = true;
            }
        }
    }
}
