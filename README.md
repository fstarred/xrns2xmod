<div data-type="ad" data-publisher="fstarred.github.io" data-format="728x90" data-zone="xrns2xmod" data-tags="renoise%2cxm%2cmod%2cconverter"></div> 

# Xrns2XMOD
Xrns2XMOD is a Renoise to MOD / XM format converter

### Wiki
For usage documentation, check the [Wiki page](https://github.com/fstarred/xrns2xmod/wiki)

### Build
1. Edit file <i>build<.bat|.sh></i>
2. Set correct <i>output_path</i> and <i>output_path_test</i> variables
3. Run <i>build<.bat|.sh></i> from <b>Developer Command Prompt</b>

### Versioning
1. Edit file <i>SharedAssemblyInfo.cs</i>
2. Set correct <i>AssemblyVersion</i> and <i>AssemblyInformationalVersion</i>
3. Save file

### Test
1. Edit <i>test.bat</i>
2. Set correct <i>output_test_path</i> and <i>nunit_console_exe</i>
3. Run <i>test.bat</i> from <b>Developer Command Prompt</b>

### Release
1. git-flow release start <version>
2. Change version
3. git-flow release finish

### Renoise schema
Renoise XML Schema song (usually named <i>RenoiseSong<version>.xsd</> can be serializated to a <i>CS</i> class by launching xsd tool:
For example:

```
xsd.exe <input.xsd> /classes /o:"/outputdir/"
```

### Donation
Xrns2XMOD is a freeware project that is developed in personal time. You can show your appreciation for this project and support future development by donating.

[![](https://camo.githubusercontent.com/f896f7d176663a1559376bb56aac4bdbbbe85ed1/68747470733a2f2f7777772e70617970616c6f626a656374732e636f6d2f656e5f55532f692f62746e2f62746e5f646f6e61746543435f4c472e676966)](https://www.paypal.me/FabrizioStellato/5)
