# proxy_modelmunge
Proxy for SWBFII's model munge. Invokes multiple instances of `pc_modelmunge` in parallel.

### Usage
Put `proxy_modelmunge.exe` from the release section into `ToolsFL/Bin/`. Then for each lvl you want to use the proxy find 
`%MUNGE_PLATFORM%_modelmunge` in it's build script and replace it with `proxy_modelmunge`, leaving the rest of the line unchanged.

As an example change this in `munge_side.bat`,
``batch
%MUNGE_PLATFORM%_modelmunge -inputfile $*.msh %MUNGE_ARGS% -sourcedir %SOURCE_DIR% -outputdir %MUNGE_DIR% 2>>%MUNGE_LOG%
```
to this.
``batch
proxy_modelmunge -inputfile $*.msh %MUNGE_ARGS% -sourcedir %SOURCE_DIR% -outputdir %MUNGE_DIR% 2>>%MUNGE_LOG%
```
