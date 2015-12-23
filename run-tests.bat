c:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild BetterModules.sln /t:Rebuild /p:Configuration=Debug /p:VisualStudioVersion=14.0

cd BetterModules.Core.Tests\bin\
nunit3-console BetterModules.Core.Tests.dll --noresult
cd ..
cd ..

cd BetterModules.Core.Database.Tests\bin\
nunit3-console BetterModules.Core.Database.Tests.dll --noresult
cd ..
cd ..

cd BetterModules.Core.Web.Tests\bin\
nunit3-console BetterModules.Core.Web.Tests.dll --noresult
cd ..
cd ..