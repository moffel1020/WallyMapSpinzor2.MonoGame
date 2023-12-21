using System.Xml.Linq;
using WallyMapSpinzor2;

string brawlPath = args[0];
string fromPath = args[1];

//read. use MapUtils.FixBmg on the content to fix xml non-compliances on a few maps
FileStream fromFile = new(fromPath, FileMode.Open, FileAccess.Read);
using StreamReader fsr = new(fromFile);
XDocument document = XDocument.Parse(MapUtils.FixBmg(fsr.ReadToEnd()));
if (document.FirstNode is not XElement element) return;
fsr.Close();
//deserialize
LevelDesc levelDesc = element.DeserializeTo<LevelDesc>();
//create window
using WallyMapSpinzor2.MonoGame.BaseGame game = new(brawlPath, levelDesc);
//run
game.Run();
