using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoD2_Editor
{
    public partial class Form1 : Form
    {
        private LinkLabel _selectedEnclaveLink = null;
        private void UpdateEnclaveList(EnclaveManager enclaveManager)
        {
            int currentCount = flowEnclaves.Controls.Count;
            int newCount = enclaveManager.NumEnclaves;

            while (currentCount < newCount)
            {
                var link = new LinkLabel
                {
                    AutoSize = true,
                    LinkColor = Color.LightBlue,
                    Margin = new Padding(3),
                    Cursor = Cursors.Hand,
                    BackColor = flowEnclaves.BackColor
                };
                int index = currentCount;
                link.Click += (s, e) => OnEnclaveLinkClicked((LinkLabel)s, enclaveManager.Enclaves[index]);
                flowEnclaves.Controls.Add(link);
                currentCount++;
            }

            while (currentCount > newCount)
            {
                flowEnclaves.Controls.RemoveAt(currentCount - 1);
                currentCount--;
            }

            for (int i = 0; i < newCount; i++)
            {
                try
                {
                    var link = (LinkLabel)flowEnclaves.Controls[i];
                    string newName = enclaveManager.Enclaves[i].DisplayName;

                    if (link.Text != newName)
                        link.Text = newName;

                    if (link == _selectedEnclaveLink)
                        link.BackColor = Color.DarkRed;
                    else
                        link.BackColor = flowEnclaves.BackColor;

                    if (newName == currEnclave.DisplayName)
                        link.BackColor = Color.DarkGreen;
                }
                catch (Exception ex) { Console.WriteLine($@"UpdateEnclaveList:  {ex}"); }
            }
        }

        private void OnEnclaveLinkClicked(LinkLabel clickedLink, Enclave enclave)
        {
            if (_selectedEnclaveLink != null && _selectedEnclaveLink != clickedLink)
            {
                _selectedEnclaveLink.BackColor = flowEnclaves.BackColor;
            }

            clickedLink.BackColor = Color.DarkRed;
            _selectedEnclaveLink = clickedLink;
            txtEnclaveAddress.Text = enclave.BaseAddress.ToString("X");

            txtCharacterAddress.Text = "0";
            lblInventory.Text = "";
            lblCharacterTraits.Text = "";
            lblCharactersLabel.Text = "";
            lblEnclavesLabel.Text = "";
            lblEquipment.Text = "";
            lblMelee.Text = "";
            lblRanged.Text = "";
            lblSidearm.Text = "";
            lblInventory.Text = "";

            _characterDetailRows.Clear();
            _characterSkillRows.Clear();
            _inventoryRows.Clear();
            _meleeWeaponDetailRows.Clear();
            _rangedWeaponDetailRows.Clear();

            tlpEnclavesCharactersDetails.Controls.Clear();

            txtEnclavesCharactersInventoryAddress.Text = "0";
            tlpEnclaveCharactersInventory.Controls.Clear();
            tlpEnclavesCharactersSkills.RowCount = 0;
            tlpMeleeWeaponStats.Controls.Clear();
            tlpRangedWeaponStats.Controls.Clear();
            tlpSideArmWeaponStats.Controls.Clear();



            foreach (Control ctrl in flowEnclaveCharacters.Controls)
            {
                if (ctrl is LinkLabel link)
                {
                    link.BackColor = Color.Transparent;
                }
            }
        }

    }
}
