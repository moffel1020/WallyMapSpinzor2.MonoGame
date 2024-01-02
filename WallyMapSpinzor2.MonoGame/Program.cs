using System.Xml.Linq;
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
string ldPath = args[1];

IDrawable drawable;
if(args.Length >= 4)
{
    string ltPath = args[2];
    string lstPath = args[3];
    LevelDesc ld = DeserializeFromPath<LevelDesc>(ldPath);
    LevelTypes lt = DeserializeFromPath<LevelTypes>(ltPath);
    LevelSetTypes lst = DeserializeFromPath<LevelSetTypes>(lstPath);
    drawable = new Level(ld, lt, lst);
}
else
{
    drawable = DeserializeFromPath<LevelDesc>(ldPath);
}

//create window
using WallyMapSpinzor2.MonoGame.BaseGame game = new(brawlPath, drawable);
//run
game.Run();
