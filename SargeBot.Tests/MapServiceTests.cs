using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using SargeBot.Features.GameData;
using SargeBot.Features.GameInfo;
using SC2APIProtocol;
using SC2ClientApi;
using System;
using System.Text;

namespace SargeBot.Tests;

[TestClass]
public class MapServiceTests
{
    private GameClient? _gameClient;
    private IServiceProvider _serviceProvider;
    private GameData? _gameData;

    [TestCase("Hardwire AIE", ExpectedResult = true)]
    public bool testSimpleResponse(string mapname)
    {
        MapService service = new MapService(new MapData());
        Response dummy = createDummyResponse();
        MapData test = service.PopulateMapData(dummy.GameInfo);

        return mapname == test.MapName;
    }

    private Response createDummyResponse()
    {
        Response dummyResponse = new Response();
        string mapName = "Hardwire AIE";
        string localMapPath = "HardwireAIE.SC2Map";
        string PathingData = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAZmAABmYAAAAAAAAAAAAAAP / wAA//AAAAAAAAAAAAAAH/+AAf/5mYAAAAAAAAAAAD//wAP////AAAAAAAAAAAB//+AH////4AfAAAAAAAAA///wD/////AP/AAAAAAAAf//8B/////4H//AAAAAAAP//+Af////+D//8AAABmZn///gH/////x///gAAA//8///8B/////+///8AAAf//P///Af/////////gAAP//n///gH/////////4AAH//5///4B////9////+AAD///P///AH//+Y/////wAB///z///wB///AH////8AA///5///8Af//gA/////AAP//+f///AH//4Af////wAB///z///4D///AH////8AAf//+Y///B///wA/////AAP///wf//4///4AH////wAD///+O//////8AA////4AAf////P/////+AAH///+AAH////j//////AZg////AAD////4ef////gP8P///AAA////+DD////4H/n///gAAH////wAf////D/8///wAAB////+AH////5//n//4AAA/////wD//5////8//8AAAP////8A//8P////n/+AAAB////+AH/+B////8f/AAAAf////AB//gP////D/wwAAP////gA//8B////gf+fgAD////wAf//gP///4H//8AAf//+YAH//8B////B///gAD///ADj///gf///wf//8AAf//gB9///8P///4H///gAD//wAf////n///+D///8AAf/4AH//D//////w////gAD/8HP/4Af/////8P///4AAf+D//+AD/////+D///+AAD/B///gA//////B////gAAfk///4Af//n//gf///4AADzP//+AP//w/+QP///+AAAZn///gH//4H/AD////gAAAD///4D//8B/wA////4AAAB///+Af/+A/+AP///+AAAAf///gH//gf/wH////gAAAH///4D//8P/+f////wAAAB///+B///n///////4AAAAP///Af//////////8AAAAB///wP//////////+AAAAAf//8H///////////gAAAAD////////////+//4AAAAADv///////n///Af+AAAAAAB///////w///gD/gAAAAAAP///H//4H//4B/4AAAAAAD///g//8B//8A/8AAAAAAA///wH/+Af/yA//AAAAAAAP//8B//AP/4Af7wAAAAAAP//+A//gH/8AP88AAAZgAH///Af/4H/+AH+OYAAP8AD///gP/+D//gD/hPAAH/gA///wH//x//8B/4H4AD/8Af//8B//+//+A//D/AB//mP///A//////Af/5/4A///////4f/j///AP////AP/////////wf//gD////wB/////////4D//wAf///4Af///+////8A//8AH///+AP////H///8AP//AD////wD////g///+AD//wA////8Af///4H///AA//+AH///+AH///+AH//wAP//+B////gD////wA//8AH///wf///8A////8AP//AD///+P////AH///+AD//wD////3////gB////gA//8B/////////4A////8Af//g//////////AP////AP//8f/h///////wB/5//gP/////wP///Gf/4AP8P/wH//3//4D///gD/8AB+B/4D//4//+A///wAf+AAPIf8Af/8H//Af//8AD/AABnH+AH/+B//gP//+AAZgAADz/AD/+Af/wH///AAAAAAA9/gB//AP/4D///AAAAAAAP/wE//gH/+A///wAAAAAAD/wD//4D//wf//8AAAAAAB/4B//+B//+P///AAAAAAAf8Af//w///////4AAAAAAH/gP//+f///////cAAAAAB//3////////////8AAAAAf//////////+D///gAAAAH///////////A///4AAAAD///////////gP///AAAAB///////+f//4H///4AAAA/////n//D//8B///+AAAAf///+A//gf/+Af///gAAAH////AH/wH//gH///4AAAB////wA/4D//8B///8AAAAf///8AP+B//+Af//+ZgAAH////An/w///AH///M8AAB////gf/+f//gB///yfgAAf///4P/////wAf//4P8AAH///8H/////8AH//8H/gAB////D//////gB//OD/8AAf///w//////8P/+AB//gAD///8H///+f////gA//8AAf//+B////D///74Af//gAD///g////gf//8cAP//8AAf//4P///4D//+ABn///gAD//+B////Af//gA////8AAfn/gf///4D//wAf////AAAw/8P////Af/4AP//8fgAAAP/j////4H/+AH///H4AAAH/+f////D//wD///x/AAAD//z////5//8A/////wAAB//+f/5////+AH///44AAA///z/8P////gA///+OAAAf//+f+B////8MH//HjwAAP///D/Af////nh//g/8AAP///wZgP/////8f/4OeAAH///+AAH//////P/+DzgAB////wAD//////3H/x58AA////+AB///x///g////AAP////wA///4P//xn//ngAD////+AP//8B///8//z4AA/////gB//+AP///n8n/AAP////wAf//gD///5/n/wAD////+AP//4A///8///4AA/////xn//+AP///P//8AAH////7////4B///5//+AAB/////////+Af//+f//AAAf/////////gP///P//gAAD///3/////4D///z//wAAAf//4/////+Af//+ZmYAAAD//8H/////gH///AAAAAAAP/+B/////4D///gAAAAAAAP/AP////8A///wAAAAAAAAPgB////+AH//4AAAAAAAAAAAP////AA//8AAAAAAAAAAABmZ//gAH/+AAAAAAAAAAAAAAP/wAA//AAAAAAAAAAAAAABmYAAGZgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";
        string TerrainData = "v7+/v7+/v7+/v7+/wMC/v7++vr6/v7++v7++wL+/v7+/v7+/v7+/v7+/v7+/v7+/f39/f39/f39/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Av7/AwL/Av7+/v76+vr6/v76/v8C/vr6/v7+/wMC/vr6+vr+/v7+/wMDAv7/AwMC/v7+/v7+/v7+/v7/AwMC/v7++v7/AwL6+vr6/wL+/v7+/z7+/v8+/v7/Pv7+/v7+/f39/f39/f7+/v7+/v8+/v7/Pv7+/z7+/v7+/v7+/v7+/v7+/v7+/v7+/v76+vr+/v8DAwMC/v7/AwMHCwcC+vb29vr6+vr/Av7+/v7+/v76/v7+/v7+/v7+/v7/AwL++vr6/wMDAv76+vsC/v7+/v8/Pz7/Pz8+/z8/Pv7+/v7+/f39/f3+/v7+/v7/Pz8+/z8/Pv8/Pz7+/v7+/v7+/v7+/v7+/v7+/v7++vr++v7+/wMHAwL/AwMHAwcLCwL69vb29vr6+vr+/vr+/v7+/vr+/v7+/v7+/v7+/v7+/v7+/vr6/wMDAv7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/f39/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz7+/v8+/v7/Pv7+/z7+/v7+/v7+/v7+/v7/AwMDAwMDAv7/AwcHAvr6+vb2+vr69vr6+v7+/v7+/v7+/v7+/v7+/v7+/v7++vr+9vb6/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/f7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz7/Pz8+/z8/Pv8/Pz7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/AwL++vr6+vb6+vb29vr+/v7/Av7+/v7+/v7+/v7+/v7+/v7+/v76+v7+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/v7+/v7+/v7+/v76/v8DAv76+vr6+v7+9vb2+v7+/v8DAv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz4e/v7+/v7+/v7+/v7+/v7+/v7/AwL+/vr++v7+/v728vL2/v7+/v8C/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7++v8DAwcC/vr28vL6/v76+v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/AwMHCwb++vbu8vr+/vb2+v7+/v7+/v7+/v9+/v7/fv7+/37+/v9+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz83Lv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wMHBwL++vb2+v8C/vr6/v7+/v7+/v7/f39+/39/fv9/f37/f39+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wMHAv7++vb6/v7+/vr+/v7+/v7+/39/f39/f39/f39/f39/f39/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/PzcvJx8XDv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wL69vb+/v7++v7+/v7+/v9/f39/f39/f39/f39/f39/f39/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz83LycfFw8G/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v76+v7/Av729vr+/vr6/v7+/v7/f39/f39/f39/f39/f39/f39/fz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vr2+v8C/vr2+wL++vr+/v7+/39/f39/f39/f39/f39/f39/f38/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/PzcvJx8XDwb+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7++vb6/v769vb6+v76+v7+/v9/f39/f39/f39/f39/f39/f39/f38/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+Hz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7/LycfFw8G/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v769vb2+vby8vb6/vr6/v7/f39/f39/f39/f39/f39/f39/f39/f38/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+Hz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz89/z8/Pv7/HxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vr28vb6+vb2+vr++vr+/39/f39/f39/f39/f39/f39/f39/f39/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/37+/v7+Hz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pf39/z7+/v7/Dwb+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vr6+v8DAwL+/vr69v9/f39/f39/f39/f39/f39/f39/f39/fz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz9/f37+/v7+Hz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz39/f39/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8DAwcPCwcG+vb2/v9/f39/f39/f39/f39/f39/f39/f39/fz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz9/f37+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz39/f3+Dv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wMHDw8LCwb+9vb+/v9/f39/f39/f39/f39/f39/f39/f39/fz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz9/f37/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz39/f4eHv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wMLCwcHAv76+v7/f39/f39/f39/f39/f39/f39/f39/f39/fz8/Pz9PRz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz9/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz89/f39/h4eDv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v76+v76+vr29vb2/39/f39/f39/f39/f39/f39/f39/f39/f39/fz8/X1dPRz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pf39/f3+Hh4OHv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wMDAv729vby8vb2+vb+/39/f39/f39/f39/f39/f39/f39/f39/f39/P29nX1dPPz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz4e/h4eHz4eDh8+Hv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/AwcG/vr29vb2+vr69v7+/39/f39/f39/f39/f39/f39/f39/f39/f39/d29nXz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/h8/Pz4fPz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8DBwL++vr6+vr+/vr2/v9/f39/f39/f39/f39/f39/f39/f39/f39/f39/d28/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v4fPz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8DAv7++vr6+vr++vb/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/Pz8/Pz8/Pz8+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/z8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v76+vb2+vb29v7/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/fz8/Pz8/Pv7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vr69vb29vr6/v7/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/fz8/Pz7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vr69vb6/wL+/39/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/fz8+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/z7+/v7+/v7+/v7+/vr6+v8DAv9/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/fv7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pf8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v8/Pz7+/v7+/v7+/wL++vr6/wMG/v9/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/f37+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz39/f8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7/Pv8+/v7+/v7+/v7+/v7++vr/AwL+/v9/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz89/f39/f8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/z8/Pv7+/v7+/v7+/v76+vr6/v7/Av7/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/fv7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz89/f39/f8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v7+/v7+/z7+/v7+/v7+/v7+/vr6/v7+/v7+/39/f39/f39/f39/f39/f39/f39/f39/f39/f39/f37+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz89/f39/f8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vr+/v7+/v7+/v7+/39/f39/f39/f39/f39/f39/f39/f39/f39/f39+/v7+/v7+/v7+/y83Pz8/Pz8/Pz8/Pz8/Pz8/Pz89/f39/f8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/39/f39/f39/f39/f39/f39/f39/f39+/39/fv7+/v7+/v7+/v8fJy83Pz8/Pz8/Pz8/Pz8/Pz8/Pz89/f39/f8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vr+/v7+/v7+/v7+/39/f39/f39/f39/f39/f39/f39/fv7+/37+/v7+/v7+/v7/DxcfJy83Pz8/Pz8/Pz8/Pz8/Pz8/Pz89/f3/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/39/f39/f39/f39/f39/f39/f37+/v7+/v7+/v7+/v7+/v8HDxcfJy83Pz8/Pz8/Pz8/Pz8/Pz8/Pz89/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/39/f39/f39/f39/f39/f39+/v7+/v7+/v7+/v7+/v7+/v8HDxcfJy83Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vr6+vr6/v7+/v7+/39/f39/f39/f39/f39/fv7+/v7+/v7+/v7+/v7+/v7+/v8HDxcfJy83Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v76+vr6+vr+/v7+/v7+/39/f39/f39/f39/f37+/v7+/v7+/v7+/v7+/v7+/v7+/v8HDxcfJy7/Pz8/fz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vr6+vr6+vr2+v7+/v7+/39/f39/f39/f39+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8HDxce/v8/P39/fz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7++vr+/vr6+vr6+v7+/v7+/39/f39/f39/fz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8HDv7+/z9/P38/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v76/v7+/v7+/v76/v7+/v7+/39/f39/f38/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/f39/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz3/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7++v8DAv7+/vr/Av7+/v7+/39/f39+/z8/Pv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8/fz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz89/f3/Pz8/Pz8/Pz8/Pz8/fz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v76/wMC+vr+/v7+/v7+/v7+/39/fv8/Pz7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pf39/f3/Pz8/Pz8/Pz8/P39/fv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vr++vr6/v7+/v7+/v7+/v7+/37+/v8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz39/f39/z8/Pz8/Pz8/Pz8/P39/fv7+/v8PBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7++vr+/vr+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz89/f39/f8/Pz8/Pz8/Pz8/Pz8/P39/fv7/HxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8DAv7+/wMC/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz89/f3/Pz8/Pz8/Pz8/Pz8/Pz8/P38+/y8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8C/vr/AwL+/v7+/v7+/wMDAv7+/v8C/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz89/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8DAv7+/v7+/v7+/v8C/v76+v7/AwL+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8vNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wMC/v7+/v7/Av76+vr6/vr7AwMDAv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/HycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8C/vr6+vr6/vr+/wL+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/w8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wL+/v7+/vr69vr/AwL+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vsC/vr++vr6+vr+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcO/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/AwL++v7+/wL++v8C/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Av76/v7/AwL+/v7+/v8C/wMDAv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycu/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny7+/v7+/v7/Dwb+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wL+/wMHAv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHv7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/x8XDwb+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wMDAv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v8vNz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v8vJx8XDwb+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wL+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7/HycvNz8/Pz8/Pz8/Pz8/Pz8+/v7+/v8/PzcvJx8XDwb+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z7+/v8+/v7+/v7+/vr+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/w8XHycvNz8/Pz8/Pz8/Pz8/fz7+/v7/Pz8/PzcvJx8XDv7+/v7+/z7+/v7+/v7+/v7+/v7+/v7+/v8/Pz7/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz8/P39/fv7+/z8/Pz8/PzcvJx7+/v7+/v8/Pz7+/v7+/v7+/v7+/v7+/v7/Pz8/Pz8/Pz8+/v7+/v7+/v8DAv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz83Lv7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz9/f37+/v8/Pz8/Pz8/Pzcu/v7+/v7/Pz8/Pz7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Ny8nHv7+/v7+/v7/Bw8XHycvNz8/Pz8/f39+/v7/Pz8/Pz8/Pz8/Pv7+/v7+/z8/Pz8/Pz7+/v7+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8+/v7/Pv7+/v8PBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Lzc/Pz8/Pz8/Pz8/PzcvJx8XDv7+/v7+/v7/Bw8XHycvNz8/Pz8/fv7+/z8/Pz8/Pz8/Pz8/Pz7+/v8/Pz8/Pz8/Pz7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8+/z8/Pv7/HxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/x8nLzc/Pz8/Pz8/Pz83LycfFw8G/v7+/v7+/v7/Bw8XHycvNz8+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz7/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/y8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v8PFx8nLzc/Pz8/Pz8+/y8nHxcPBv7+/v7+/v7+/v7/Bw8XHycu/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/wcPFx8nLzc/Pz8/Pz7+/x8XDwb+/v7+/v7+/v7+/v7/Bw8XHv7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/wcPFx8nLzc/Pz8+/v7+/w8G/v7+/v7+/v7+/v7+/v7/Bw7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/wcPFx8nLzc/Pv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcO/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wcPFx8nLv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wcPFx7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wcO/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/w8G/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8vNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8fFw8G/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/HycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/LycfFw8G/v7+/v7+/v7+/v7+/v7+/v7+/v7+/w8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Pz83LycfFw8G/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v8PBv7+/v7+/v7+/v7+/v7+/wcO/v7+/z8/Pz83LycfFw8G/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7/HxcPBv7+/v7+/v7+/v7+/v8HDxce/v8/Pz8/Pz83LycfFw8G/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/y8nHxcPBv7+/v7+/v7+/v7/Bw8XHycu/z8/Pz8/Pz83LycfFw7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycu/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pv8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/z8/Ny8nHxcPBv7+/v7+/v7+/wcPFx8nLzc/Pz8/Pz8/Pz83Lyce/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHv7/Pz8+/z8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v8/Pz8/Pz8/Pz7+/v8/Pz8/Pz8/Pz8/Pz8+/v7/fz8/Pz8/Ny8nHxcPBv7+/v7+/v7/DxcfJy83Pz8/Pz8/Pz8/Pz83Lv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw7+/v7/Pv7+/z8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/v7+/v8/Pz8/Pz8+/v7+/v7/Pz8/Pz8/Pz8/Pv7+/39/fz8/Pz8/Ny8nHxcPBv7+/v7+/v7/HycvNz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v8/Pz8/Pv7+/v7+/y83Pz8/Pz8/Pz7+/v9/f38/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7/Lzc/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Av7+/v7+/v7+/z8/Pz8/Pz8/Pv7+/v7+/v7+/v7+/v7+/v8/Pz7+/v7+/v8fJy83Pz8/Pz8+/v7/f39/Pz8/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wMDAv7+/v7+/v7+/z8/Pv8/Pz7+/v7+/v7+/v7+/v7+/v7+/v8+/v7+/v7/DxcfJy83Pz8/Pv7+/v8/fz8/Pz8/Pz8/Pz8/Ny8nHxcO/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/AwMC/v7/Av7+/v7+/z7+/v8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8HDxcfJy83Pz7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Ny8nHv7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Av7+/wMC/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8HDxcfJy7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v8PBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/AwL++v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8HDxce/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7/HxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8DAv76/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8HDv7+/v7+/v8vNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/y8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wMC/v7+/vr/AwL+/v76/v7+/vr+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/HycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wMDAv8C/vr6/vr+/vr+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/w8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wMC/wL++vr6+wMC/vr+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcPBv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vr6+vb7Awb+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHxcO/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v77AwL++vr7AwL+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny8nHv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8C/vr+/vr+/vr+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Ny7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7++vr6/vr+/v7+/v7+/v7++vr+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz89/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wMC/v7+/v7+/v76+vr6+v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycu/z9/Pz8/Pz8/Pz8/Pz8/Pz8/Pf39/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vr6/vr6/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHv7/f39/Pz8/Pz8/Pz8/Pz8/Pz39/f39/z8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v76+vr6+vr+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw7+/v7/f39/Pz8/Pz8/Pz8/Pz89/f39/f8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z7+/v9+/v7+/v7+/v7+/wL/Avr29vr+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/f39/Pz8/Pz8/Pz8/Pf39/f3/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8/Pz7/f39+/v7+/v7+/v8DAv7+/vr6/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/fz8/Pz8/Pz8/Pz8/Pf39/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/fz7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Pz8+/39/f39+/v7+/v7+/v8C/vr++v8C/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pf8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/P39/fv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz9/f39/f39+/v7+/v7+/wL++vr+/v7++v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz9/P38+/v7/Dwb+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/f39/f39/f39+/v7+/v7+/wL++v7+/v7++v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/f39/Pz7+/x8XDwb+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/39/f39/f39/f39+/v7+/v7+/v76/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/fz8/Pv8vJx8XDwb+/v7+/v7+/v7+/v7+/v7+/v7+/v9/f39/f39/f39/f39+/v7+/v7+/vr+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/PzcvJx8XDwb+/v7+/v7+/v7+/v7+/v7+/v7/f39/f39/f39/f39/f39+/v7+/v7++v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/PzcvJx8XDwb+/v7+/v7+/v7+/v7+/v7+/39/f39/f39/f39/f39/f39+/v7+/v76/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz89/z8/Pz8/Pz8/Pz8/Pz8/Pz8/PzcvJx8XDwb+/v7+/v7+/v7+/v7+/v9/f39/f39/f39/f39/f39/f39+/v7+/vr+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pf39/z8/Pz8/Pz8/Pz8/Pz8/Pz8/PzcvJx8XDv7+/v7+/v7+/v9+/v7/f39/f39/f39/f39/f39/f39/f39+/v7++vr+/v7+/v76/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz39/f39/z8/Pz8/Pz8/Pz8/Pz8/Pz8/PzcvJx7+/v7+/v7+/v7/f39+/39/f39/f39/f39/f39/f39/f39/f39+/v76+v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz39/f39/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pzcu/v7+/v7+/v7+/39/f39/f39/f39/f39/f39/f39/f39/f39/f39+/vb6/v7+/v7+/vr+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz39/f39/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v9/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39++v7+/v7++vr+/v7+/v7+/v7+/v8+/v7+/v7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz39/f39/z8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/fv76/v7+/v76+vr+/v7+/v7+/v7/Pz8+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz39/f39/z8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/39/f39/f39/f39/f39/f39/f39/f39/f39/f39/f37+/vr+/v7+/v76+v7+/v7+/v7+/z7/Pv7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz39/f8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v9/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/f37+/v7+/vr6/vr+/v7+/v7+/v8/Pz7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz3/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/f37+/v7+/vr6+v7+/v7+/v7+/v8+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/z8/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39+/v7+/v769vb6+v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pv8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v8/Pz8/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/fv7++vr+/vr29vb6+v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7/Pz8/Pz8/f39/f39/f39/f39/f39/f39/f39/f39/f39/f39/fv76+vr+/vr29vr6/v76/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/z8/Pz8/Pz8/P39/f39/f39/f39/f39/f39/f39/f39/f39/f39/fvb2+v7++vb2+v7++vr6/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Ph7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz9vd39/f39/f39/f39/f39/f39/f39/f39/f39/f37+9vb/AwL+9vr6/v7++v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Ph8/Pz4e/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/X2dvd39/f39/f39/f39/f39/f39/f39/f39/f39+/v7y9v8DAv76+v8DAv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+Hz4eDh8+Hh4e/h8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/P09XX2dvP39/f39/f39/f39/f39/f39/f39/f39/f39+/vb2/wMC/vr/AwL+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v79/f4eHf39/f3/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/R09XXz8/f39/f39/f39/f39/f39/f39/f39/f39/f39+/v7/AwMDAwMHBwL+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v79/h4d/f39/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/P38/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/R08/Pz8/f39/f39/f39/f39/f39/f39/f39/f39/fv7+/wMC+v8DAwcHAv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+Hh39/f8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv9/f38/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/f39/f39/f39/f39/f39/f39/f39/f37+/v76/vr29vr+/wMDAv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+Df39/f8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v9/f38/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/f39/f39/f39/f39/f39/f39/f39/f37++vr6+vL2+vr+/wL+/wL+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v79/f39/f8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+Hv7+/v9/f38/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/f39/f39/f39/f39/f39/f39/f39/f37+/v769vb6+v7+/v8DAv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8HDv7+/v89/f3/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+Hv7+/v9+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/P39/f39/f39/f39/f39/f39/f39/f39+/wL+/vr69vb29vr+/v8C/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHv7/Pz89/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+Hv7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz9/f39/f39/f39/f39/f39/f39/f39/fv7+/vr29vr28vb2+v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wcPFx8nLv8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+Hv7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz9/f39/f39/f39/f39/f39/f39/f37+/v7++vby8vLy9vb6/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v8HDxcfJy83Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz9/f39/f39/f39/f39/f39/f39+/v7+/v769vb29vb29vr/Av7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Bw8XHycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/f39/f39/f39/f39/f39/f39/fv7+/v7/Av76+vr28vL2+v8C/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/wcPFx8nLzc/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/P39/f39/f39/f39/f39/f39/f37+/v7+/v8C/vr+/vby+vb7Av7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/DxcfJy83Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/P39/f39/f39/f39/f39/f39+/v7+/v7+/wL+/wL+/v7++v7/AwMC/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/HycvNz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/39/fv9/f37/f39+/39/fv7+/v7+/v7+/v7/AwMC/v76/v8DAwcC/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Lzc/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/37+/v9+/v7/fv7+/37+/v7+/v7+/v7++v8DAvr6+vr+/wMDAwL++v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/vr6+vr69vb2+wMDAv8C/v76/v7+/v7+/v7+/v7+/v7+/v7+/v7+/z8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7++vr69vb2+vr6/v7+/v76+vr6/wMDAwMC/v7+/v7+/v7+/v7+/v7+/h8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v76+vr2+vr++vr6+v7++vr6+vr6/v8DAwMC/v7+/v7+/v7+/v7+/v7+/v8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v7+/v8DAv76+v7+/v7+/v7+/v7+/v7+/v7++vr6/v7+/v7++vb29vb29vr6/wMC/v7/Av7+/v7+/v7+/v7+/v7+/v8/Pz7/Pz8+/z8/Pv8/Pz8/Pz8/Pz8/Pz8/Pz7+/v7+/v3+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pz8+/v7+/v7+/v7/AwL+/v7++v7+/v7+/v7+/v7+/v7+/v7++v8C/v7+/vr29vb28vL2/v8DBv7+/v7+/v7+/v7+/vr6+v7+/v7+/v8+/v7/Pv7+/z7+/v8/Pz8/Pz8/Pz8/Pz8+/v7+/v79/f3+/v7+/v7/Pz8/Pz8/Pz8/Pz8/Pv7+/v7+/v7+/v8DAwL/Av76+v7+/v7+/v7+/v7+/v7+/v7+/v8DAvr29vL29vb29vr/AwcC/vr6/v7+/v7+/v7++vr6/v7+/v7+/v7+/v7+/v7+/v7+/v8/Pz7/Pz8+/z8/Pv7+/v7+/f39/f3+/v7+/v7/Pz8+/z8/Pv8/Pz7+/v7+/v7+/v7/AwL+/vr6/v7+/v7+/v7+/v7+/v7+/v7+/v7+/v7++vb2+vr6+vr/AwcHAv7++vr+/wL++v7+/v7++vr+/v7+/v7+/v7++vr+/v7+/v7+/v8+/v7/Pv7+/z7+/v7+/v39/f39/f3+/v7+/v7/Pv7+/z7+/v8+/v7+/v7+/wMC+v7+/v7+/wMC/v7+/v7+/v7+/v7+/v78=";
        string PlacementData = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAZmAABmYAAAAAAAAAAAAAAP/wAA//AAAAAAAAAAAAAAH/+AAf/5mYAAAAAAAAAAAD//wAP////AAAAAAAAAAAB//+AH////4AfAAAAAAAAA///wD/////AP/AAAAAAAAf//8B/////gH//AAAAAAAP//+Af////wD//8AAABmZn///gH////4B///gAAA//8///8B////8A///8AAAf//P///Af///+AP///gAAP//n///gH////AH///4AAH//5///4B////wD///+AAD///P///AH//+YB////wAB///z///wB///AA////8AA///5///8Af//gAf////AAP//+f///AH//4Af////wAB///z///4D///AH////8AAf//+Yf//B///wA/////AAP///wD//4///4AH////wAD///+A//////8AA////4AAf///wP/////+AAH///+AAH///8D//////AZg////AAD////gef////gP8P///AAA////+DD////4H/n///gAAH////wAf////D/8///wAAB////+AH////5//n//4AAA/////wD//5////8//8AAAP////8A//8P////n/+AAAB////+AH/+B////8f/AAAAf////AB//gP////D/wwAAP////gA//8B////gf+fgAD////wAH//gP///4H//8AAf//+YAA//8B////B///gAD///ADgH//gf///wf//8AAf//gB8A//8P///4H///gAD//wAfAH//n///+D///8AAf/4AH4AD//////w////gAD/8HP/AAf/////8P///4AAf+D//4AD/////+D///+AAD/B///AA//////B////gAAfk///4Af//n//gf///4AADzP//+AP//w/+QP///+AAAZn///gH//4H/AD////gAAAD///4D//8B/wAf///4AAAB///+Af/+A/+AD///+AAAAf///gH//gf/wAf///gAAAH///4D//8P/+QD///wAAAB///+Af//j//8Af//4AAAAP///AD///f//gD//8AAAAB///wAf//7//8Af/+AAAAAf//8AD///f//gD//gAAAAD///8Af//7//8A//4AAAAADv//gD///H//gAf+AAAAAAB//8A///w//8AB/gAAAAAAP//gH//4H//gAP4AAAAAAD//8A//8Af/8AB8AAAAAAA///gH/+AD/yAAPAAAAAAAP//8B//AAf4AQDwAAAAAAP//+A//gAD8AOA8AAAZgAH///Af/gEAeAHwOYAAP8AD///gP/wDgDgD+BPAAH/gAf//wB/4B8AcB/4H4AD/8AD//8AP8A/gCA//D/AB//mAf//AB/AP8AAf/5/4A///wD//4APgH/gAP////AP//8Af//wBwD/8AD////wB///gD//+AIB//gAf///4Af//8A///wAA//8AH///+AP///gH//+AAP//AD////wD///8A///wAD//wA////8Af///gH//+AA//+AH///+AH///+AH//wAH//+Af///gD////wA//8AA///wD///8A////8AP//AAH//+Af///AH///+AD//wAA///wD///gB////gAf/4BAH//8Af//4A////8AD/8A4A///gD///AP////AAf+AfAB//8A///wB/5//gAD/AP4AP//gGf/4AP8P/wEAfwD/AD//8AD/8AB+B/4DgD4B/4A///gAf+AAPIH8AcAcA//Af//8AD/AABnA+AHgCAf/gP//+AAZgAADwHAD8AAf/wH///AAAAAAA8AgB/gAP/4D///AAAAAAAPAAE/8AH/+Af//wAAAAAAD4AD//gD//wD//8AAAAAAB/AAf/+B//+Af//AAAAAAAf4AD//w///wD//4AAAAAAH/gAf/+P//8Af//cAAAAAB//wD//9///gD///8AAAAAf/8Af//v//8AD///gAAAAH//gD//9///gA///4AAAAD//8Af//v//8AP///AAAAB///gD//8f//gH///4AAAA///8An//D//8B///+AAAAf///gA//gf/+Af///gAAAH///8AH/wH//gH///4AAAB////gA/4D//8B///8AAAAf///8AP+B//+Af//+ZgAAH////An/w///AH///M8AAB////gf/+f//gB///yfgAAf///4P/////wAP//4P8AAH///8H/////8AB//8H/gAB////D//////gAP/OD/8AAf///w//////8AB+AB//gAD///8H///+f/+APgA//8AAf//+B////D//wD4Af//gAD///g////gf/+AcAP//8AAf//4P///4D//wABn///gAD//+B////Af/+AA////8AAfn/gf///4D//wAf////AAAw/8P////Af/4AP////gAAAP/j////4H/+AH////4AAAH/+f////D//wD/////AAAD//z////5//8A/////wAAB//+f/5////+AH////4AAA///z/8P////gA////+AAAf//+f+B////8MH////wAAP///D/Af////ngf///8AAP///wZgP/////8D///+AAH///+AAH//////A////gAB////wAD//////wH///8AA////+AB///x//8A////AAP////wA///4P//hn///gAD////+AP//8B///8///4AA/////gB//+AP///n///AAP////gAf//gD///5///wAD////wAP//4A///8///4AA////4Bn//+AP///P//8AAH///8A////4B///5//+AAB///+AP///+Af//+f//AAAf///AH////gP///P//gAAD///wD////4D///z//wAAAf//4B////+Af//+ZmYAAAD//8A/////gH///AAAAAAAP/+Af////4D///gAAAAAAAP/AP////8A///wAAAAAAAAPgB////+AH//4AAAAAAAAAAAP////AA//8AAAAAAAAAAABmZ//gAH/+AAAAAAAAAAAAAAP/wAA//AAAAAAAAAAAAAABmYAAGZgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";

        StartRaw start = new()
        {
            MapSize = new Size2DI{ X = 128, Y = 148 },
            PlayableArea = new RectangleI{ P0 = new PointI{ X = 0, Y = 0 }, P1 = new PointI { X = 128, Y = 148 } },
            PathingGrid = new ImageData{ BitsPerPixel = 1, Size = new Size2DI { X = 128, Y = 148 },
                Data = Google.Protobuf.ByteString.CopyFrom(Encoding.Unicode.GetBytes(PathingData)) },
            TerrainHeight = new ImageData{ BitsPerPixel = 8, Size = new Size2DI{ X = 128, Y = 148 }, Data = Google.Protobuf.ByteString.CopyFrom(Encoding.Unicode.GetBytes(TerrainData)) },
            PlacementGrid = new ImageData{ BitsPerPixel = 1, Size = new Size2DI{ X = 128, Y = 148 }, Data = Google.Protobuf.ByteString.CopyFrom(Encoding.Unicode.GetBytes(PlacementData)) }
        };
        InterfaceOptions options = new InterfaceOptions
        {
            Raw = true,
            Score = true,
            ShowCloaked = true,
            RawAffectsSelection = true,
            RawCropToPlayableArea = true,
            ShowPlaceholders = false,
            ShowBurrowedShadows = true
        };

        ResponseGameInfo responseGameInfo = new ResponseGameInfo { StartRaw = start, Options = options, MapName = mapName, LocalMapPath = localMapPath };
        dummyResponse.GameInfo = responseGameInfo;
        return dummyResponse;
    }
}
