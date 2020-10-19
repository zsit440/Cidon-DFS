csc /r:Nancy.dll /r:Nancy.Hosting.Self.dll node.cs 
csc /r:Nancy.dll /r:Nancy.Hosting.Self.dll -out:net.exe net.cs

set config=config8.txt


start "node1" cmd /k node.exe %config% 1 2 4 ^ > node1.log
start "node2" cmd /k node.exe %config% 2 1 3 4 ^ > node2.log
start "node3" cmd /k node.exe %config% 3 2 5 6 4 ^ > node3.log
start "node4" cmd /k node.exe %config% 4 1 2 3 5 ^ > node4.log
start "node5" cmd /k node.exe %config% 5 3 6 4 ^ > node5.log
start "node6" cmd /k node.exe %config% 6 3 5 7 8^ > node6.log
start "node7" cmd /k node.exe %config% 7 5 6 ^ > node7.log
start "node8" cmd /k node.exe %config% 8 3 6 ^ > node7.log
net.exe %config% > net.log
pause