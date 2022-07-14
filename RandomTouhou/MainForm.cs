using System.Media;

namespace RandomTouhou
{
    public partial class MainForm : Form
    {
        private readonly Random _rng = new Random();
        private readonly List<int> _selected = new List<int>();
        private readonly CheckBox[] _gameCheckBoxList;
        private readonly Bitmap[] _gameCoverList;
        private readonly string[] _gameTitleList;
        private readonly SoundPlayer _selectSFX, _okSFX;

        public MainForm()
        {
            InitializeComponent();

            _gameCheckBoxList = new CheckBox[]
            {
                CbTh01,
                CbTh02,
                CbTh03,
                CbTh04,
                CbTh05,
                CbTh06,
                CbTh07,
                CbTh075,
                CbTh08,
                CbTh09,
                CbTh095,
                CbTh10,
                CbTh105,
                CbTh11,
                CbTh12,
                CbTh123,
                CbTh125,
                CbTh128,
                CbTh13,
                CbTh135,
                CbTh14,
                CbTh143,
                CbTh145,
                CbTh15,
                CbTh155,
                CbTh16,
                CbTh165,
                CbTh17,
                CbTh175,
                CbTh18
            };
            _gameCoverList = new Bitmap[]
            {
                Properties.Resources.th01,
                Properties.Resources.th02,
                Properties.Resources.th03,
                Properties.Resources.th04,
                Properties.Resources.th05,
                Properties.Resources.th06,
                Properties.Resources.th07,
                Properties.Resources.th075,
                Properties.Resources.th08,
                Properties.Resources.th09,
                Properties.Resources.th095,
                Properties.Resources.th10,
                Properties.Resources.th105,
                Properties.Resources.th11,
                Properties.Resources.th12,
                Properties.Resources.th123,
                Properties.Resources.th125,
                Properties.Resources.th128,
                Properties.Resources.th13,
                Properties.Resources.th135,
                Properties.Resources.th14,
                Properties.Resources.th143,
                Properties.Resources.th145,
                Properties.Resources.th15,
                Properties.Resources.th155,
                Properties.Resources.th16,
                Properties.Resources.th165,
                Properties.Resources.th17,
                Properties.Resources.th175,
                Properties.Resources.th18
            };
            _gameTitleList = new string[]
            {
                 "Touhou 1 - The Highly Responsive to Prayers",
                 "Touhou 2 - The Story of Eastern Wonderland ",
                 "Touhou 3 - Phantasmagoria of Dim.Dream",
                 "Touhou 4 - Lotus Land Story",
                 "Touhou 5 - Mystic Square",
                 "Touhou 6 - The Embodiment of Scarlet Devil",
                 "Touhou 7 - Perfect Cherry Blossom",
                 "Touhou 7.5 - Immaterial and Missing Power",
                 "Touhou 8 - Imperishable Night",
                 "Touhou 9 - Phantasmagoria of Flower View",
                 "Touhou 9.5 - Shoot the Bullet",
                 "Touhou 10 - Mountain of Faith",
                 "Touhou 10.5 - Scarlet Weather Rhapsody",
                 "Touhou 11 - Subterranean Animism",
                 "Touhou 12 - Undefined Fantastic Object",
                 "Touhou 12.3 - Touhou Hisoutensoku",
                 "Touhou 12.5 - Double Spoiler",
                 "Touhou 12.8 - Great Fairy Wars",
                 "Touhou 13 - Ten Desires",
                 "Touhou 13.5 - Hopeless Masquerade",
                 "Touhou 14 - Double Dealing Character",
                 "Touhou 14.3 - Impossible Spell Card",
                 "Touhou 14.5 - Urban Legend in Limbo",
                 "Touhou 15 - Legacy of Lunatic Kingdom",
                 "Touhou 15.5 - Antinomy of Common Flowers",
                 "Touhou 16 - Hidden Star in Four Seasons",
                 "Touhou 16.5 - Violet Detector",
                 "Touhou 17 - Wily Beast and Weakest Creature",
                 "Touhou 17.5 - Touhou Gouyoku Ibun",
                 "Touhou 18 - Unconnected Marketeers"
       };
            _selectSFX = new SoundPlayer(Properties.Resources.se_select00);
            _okSFX = new SoundPlayer(Properties.Resources.se_extend);
        }

        private void BtnRoll_Click(object sender, EventArgs e)
        {
            // Clear list
            _selected.Clear();
            // Add checked touhou to the list
            for (int i = 0; i < _gameCheckBoxList.Length; i++)
                if (_gameCheckBoxList[i].Checked)
                    _selected.Add(i);

            // Handle cases
            switch (_selected.Count)
            {
                // No game selected
                case 0:
                    MessageBox.Show("Pick a game.", "Hey", MessageBoxButtons.OK);
                    break;
                // One game selected
                case 1:
                    LbResult.Text = _gameTitleList[_selected[0]];
                    PbTouhou.Image = _gameCoverList[_selected[0]];
                    break;
                default:
                    // Clear previous result
                    LbResult.Text = "";
                    // Disable button
                    BtnRoll.Enabled = false;
                    // Roll!
                    if (!BWRoller.IsBusy)
                        BWRoller.RunWorkerAsync();
                    break;
            }
        }

        private void BWRoller_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int r = _rng.Next(_selected.Count), i = r;
            int delay = 120;
            
            for (int count = 0; count < 1000; count++)
            {
                // next game is always different from the previous game
                while (_selected.Count > 1 && i == r)
                    r = _rng.Next(_selected.Count);
                i = r;

                // Update result
                BWRoller.ReportProgress(0, i);

                // Roll speed
                if (count > 2200 / delay)
                    delay = (int)(delay * 1.14);
                // Roll stop
                if (delay > 1000)
                {
                    e.Result = i;
                    break;
                }

                Thread.Sleep(delay);
            }
        }

        private void BWRoller_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.UserState == null)
                throw new Exception("Yukari did something nasty.");

            // randomly select touhou in the list
            PbTouhou.Image = _gameCoverList[_selected[(int)e.UserState]];
            _selectSFX.Play();
        }

        private void BWRoller_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
                throw new Exception("Yukari did something nasty.");

            // Set result
            LbResult.Text = _gameTitleList[_selected[(int)e.Result]];
            // Enable button
            BtnRoll.Enabled = true;
            Task.Run(() =>
            {
                Thread.Sleep(60);
                _okSFX.Play();
            });
        }
    }
}