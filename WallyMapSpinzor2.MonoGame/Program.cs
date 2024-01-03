using System.Xml.Linq;
using System.IO;
using WallyMapSpinzor2;

static T DeserializeFromPath<T>(string fromPath) 
    where T : IDeserializable, new()
{
    FileStream fromFile = new(fromPath, FileMode.Open, FileAccess.Read);
    using StreamReader fsr = new(fromFile);
    XDocument document = XDocument.Parse(MapUtils.FixBmg(fsr.ReadToEnd()));
    if(document.FirstNode is not XElement element) throw new ArgumentException($"File {fromPath} does not contain XElement");
    fsr.Close();
    return element.DeserializeTo<T>();
}

string brawlPath = args[0];
string dumpPath = args[1];
string fileName = args[2];

LevelDesc ld = DeserializeFromPath<LevelDesc>(Path.Join(dumpPath, "Dynamic", fileName).ToString());
LevelTypes lt = DeserializeFromPath<LevelTypes>(Path.Join(dumpPath, "Init", "LevelTypes.xml").ToString());
LevelSetTypes lst = DeserializeFromPath<LevelSetTypes>(Path.Join(dumpPath, "Game", "LevelSetTypes.xml").ToString());
IDrawable drawable = new Level(ld, lt, lst);

//create window
using WallyMapSpinzor2.MonoGame.BaseGame game = new(brawlPath, drawable);
//run
game.Run();
