using System.Xml.Linq;
using Microsoft.Xna.Framework;
using WallyMapSpinzor2;

string brawlPath = args[0];
string fromPath = args[1];

//read. use MapUtils.FixBmg on the content to fix xml non-compliances on a few maps
FileStream fromFile = new(fromPath, FileMode.Open, FileAccess.Read);
using StreamReader fsr = new(fromFile);
XDocument document = XDocument.Parse(MapUtils.FixBmg(fsr.ReadToEnd()));
if (document.FirstNode is not XElement element) return;
fsr.Close();
//write to file.
LevelDesc levelDesc = element.DeserializeTo<LevelDesc>();

using WallyMapSpinzor2.MonoGame.BaseGame game = new(brawlPath, levelDesc);
game.IsMouseVisible = true;
game.Window.AllowUserResizing = true;
game.Window.Title = "WallyMapSpinzor2.MonoGame";
game.Run();
