set config=config4.txt

csc /r:Nancy.dll /r:Nancy.Hosting.Self.dll node.cs 
csc /r:Nancy.dll /r:Nancy.Hosting.Self.dll -out:net.exe net.cs

start "node1" cmd /c node.exe %config% 1 2 3 4 ^ > node1.log
start "node2" cmd /c node.exe %config% 2 1 3 ^ > node2.log
start "node3" cmd /c node.exe %config% 3 1 2 4 ^ > node3.log
start "node4" cmd /c node.exe %config% 4 1 3 ^ > node4.log



start "net" cmd /k net.exe %config% > net.log
Pause
