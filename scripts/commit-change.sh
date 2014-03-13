cd ThirdParty/MonoGame-SDL2/
git commit -a
git branch --set-upstream-to=origin/mgsdl2-glshader
git pull
git push origin mgsdl2-glshader
cd ../..

scripts/format-code.sh 2>/dev/null
scripts/rewrite-csproj.pl
git commit -a
git pull
git push origin master
