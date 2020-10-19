using Nancy;
using Nancy.Hosting.Self;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Linq;
namespace DavidNet{

     public class PriorityQueue<T> where T : IComparable<T>
  {
    private List<T> data;

    public int boundary = -1;
    public PriorityQueue()
    {
      this.data = new List<T>();
    }

    public void Enqueue(T item)
    {
      data.Add(item);
      int ci = data.Count - 1; // child index; start at end
      while (ci > 0)
      {
        int pi = (ci - 1) / 2; // parent index
        if (data[ci].CompareTo(data[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
        T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
        ci = pi;
        //Console.WriteLine(Node.traffic.ToString());
      }
    }

    public T Dequeue()
    {
      // assumes pq is not empty; up to calling code
      int li = data.Count - 1; // last index (before removal)
      T frontItem = data[0];   // fetch the front
      data[0] = data[li];
      data.RemoveAt(li);

      --li; // last index (after removal)
      int pi = 0; // parent index. start at front of pq
      while (true)
      {
        int ci = pi * 2 + 1; // left child index of parent
        if (ci > li) break;  // no children so done
        int rc = ci + 1;     // right child
        if (rc <= li && data[rc].CompareTo(data[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
          ci = rc;
        if (data[pi].CompareTo(data[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
        T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
        pi = ci;
      }
       //Console.WriteLine(Node.traffic.ToString());
      return frontItem;
    }

    public T Peek()
    {
      T frontItem = data[0];
      return frontItem;
    }

    public int Count()
    {
      return data.Count;
    }

    public override string ToString()
    {
      string s = "";
      for (int i = 0; i < data.Count; ++i)
        s += data[i].ToString() + " ";
      s += "count = " + data.Count;
      return s;
    }

    public bool IsConsistent()
    {
      // is the heap property true for all data?
      if (data.Count == 0) return true;
      int li = data.Count - 1; // last index
      for (int pi = 0; pi < data.Count; ++pi) // each parent index
      {
        int lci = 2 * pi + 1; // left child index
        int rci = 2 * pi + 2; // right child index

        if (lci <= li && data[pi].CompareTo(data[lci]) > 0) return false; // if lc exists and it's greater than parent then bad.
        if (rci <= li && data[pi].CompareTo(data[rci]) > 0) return false; // check the right child too.
      }
      return true; // passed all checks
    } // IsConsistent
  } // PriorityQueue


    public class MsgBlock: IComparable<MsgBlock>{
        public string time = "";
        public string from = "";
        public string to = "";
        public string tok = "";
        public string pay = "";
        public int logicalTime;
        

        public MsgBlock(string[] message){
            this.time = message[0];
            this.from = message[1];
            this.to = message[2];
            this.tok = message[3];
            this.pay = message[4];
            logicalTime = Int32.Parse(message[0]); 
        }
        public void addTime(int time){
            this.logicalTime += time;
            this.time = this.logicalTime.ToString();
        }
        public int CompareTo(MsgBlock other){
            if (this.logicalTime < other.logicalTime) return -1;
            else if (this.logicalTime > other.logicalTime) return 1;
            else{
                if (String.Equals(this.tok, other.tok)){return 0;}
                else if(String.Equals(this.tok, "2") && string.Equals(other.tok, "1")){return -1;}
                else if(String.Equals(this.tok, "2") && string.Equals(other.tok, "3")){return -1;}
                else if((String.Equals(this.tok, "1") && string.Equals(other.tok, "3")) || (String.Equals(this.tok, "3") && string.Equals(other.tok, "1"))){return 0;}
                else {return 1;}
            }
             
        }
        public override string ToString(){
            return $"{time} {from} {to} {tok} {pay}";
        }

        public string[] ToArray(){
            return $"{time} {from} {to} {tok} {pay}".Split();
        }
    }

    public static class Node {

        public static string nodeNum = "0";
        public static Dictionary<string,int> confData = new Dictionary<string,int>();
        public static string myPort;

        //public static int curr_Time = 0;
        
        public static PriorityQueue<MsgBlock> traffic = new PriorityQueue<MsgBlock>();

        //public static Semaphore wait_sem = new Semaphore(0,1); 

        //public static SortedList<int,string[]> msg_buffer = new SortedList<int, string[]>(new DuplicateKeyComparer<int>());

        public static object mylock =  new object();
    }

 
        


    class DavidNet
        {

        
        static void Main (string[] args) {
        
		HostConfiguration hostConfigs = new HostConfiguration(){
			UrlReservations = new UrlReservations() {CreateAutomatically = true}
		};
		
		
		ReadFile(args[0]);
        //Console.WriteLine(Node.confData["-"]);
        Node.myPort = Node.confData["0"] + "";
        using (var host = new NancyHost(new Uri("http://localhost:" + Node.myPort),new DefaultNancyBootstrapper(), hostConfigs)) {
            host.Start();
            
            Console.WriteLine("Running on http://localhost:"+Node.myPort); //Start sending message;
            string portNumber = Node.confData["1"].ToString();
            string ThisNode = Node.nodeNum;
            string time = "0";
            string from = "0";
            string to = "1";
            string tok = "1";
            string pay = "0";
            string message =  string.Format("{0} {1} {2} {3} {4}",time, from, to, tok, pay );
            DavidNet.sendData("http://localhost:"+portNumber,message);
            Console.WriteLine($"... 0 0 < {from} {to} {tok} {pay}");
            
            /*var startTimeSpan = new TimeSpan(0);
            var periodTimeSpan = TimeSpan.FromSeconds(5);

          var timer = new System.Threading.Timer((e) =>
          {
            DeliverMessage();   
            }, null, startTimeSpan, periodTimeSpan);*/
            Console.ReadLine();
            
        }

        //Console.WriteLine("The file name is: " + confData[2]);	
		//Console.ReadLine();
        }


    public static void ReadFile(string filename){
        string[] configFile = File.ReadAllLines(filename);
        int counter = 0;
        foreach (string fline in configFile){
            if (fline.Contains("//")){
                int index = fline.IndexOf("//");
                if (index != 0){
                configFile[counter] =  fline.Substring(0,index);
                }
                else{
                    configFile[counter] = "";
                }
                
            }
            if (configFile[counter] != ""){
                string[] pair = configFile[counter].Split();
                string key = pair[0];
                int value = Int32.Parse(pair[1]);  
                Node.confData.Add(key,value);
            }
            //Console.WriteLine(configFile[counter]);
            counter ++;
           
        }
        //return confData;
    }
    


        public static async void sendData(string url,string message)
        {
            WebClient client = new WebClient();
            var data = await client.UploadStringTaskAsync(new Uri(url),"post",message);
            //var data = client.UploadString(new Uri(url),"post",message);
            client.Dispose();
           // client.Headers.Add ("user-agent", "Your User-Agent");
    // Do your operations here...
            return;
            
        }

        public static int getDelay(string source, string destination){
            string curr_path = source + "-" + destination;
            foreach (string path in Node.confData.Keys){
                if (string.Equals(path,curr_path)){
                    return Node.confData[curr_path];
                }
            }
            return Node.confData["-"];
        }
        //Console.WriteLine(uri.GetType());
       public static void NoDelayTest(string[] data_list){
            
            //data_list[0] = data_list[0].Substring(0,data_list[0].Length -1);
           
            string message = String.Join(" ",data_list);
            string destPort = Node.confData[data_list[2]].ToString();
            DavidNet.sendData("http://localhost:"+destPort,message);
            Console.WriteLine($"... {data_list[0]} 0 < {data_list[1]} {data_list[2]} {data_list[3]} {data_list[4]}");
       }

       /* public static int getBoundary(){
          foreach(MsgBlock msg in Node.traffic){
              if (String.Equals(msg.tok, "1") || String.Equals(msg.tok, "3"))
                  return msg.logicalTime;
          }
          return -1;
       }*/

       public static void DeliverMessage(){
           if (Node.traffic.boundary < 0){
               return;
           }
           
           MsgBlock msg; 
           bool pass = true;
           while(Node.traffic.Count() > 0 && pass){
               msg = Node.traffic.Peek();
               if (msg.logicalTime > Node.traffic.boundary){
                   break;
               }
               if (String.Equals(msg.tok,"1") || String.Equals(msg.tok,"3") ){
                   //boundary = msg.logicalTime;
                   Node.traffic.boundary = -1;
                   pass = false;
               }
               msg = Node.traffic.Dequeue();
               string destPort = Node.confData[msg.to].ToString();
               string message = msg.ToString();
               DavidNet.sendData("http://localhost:"+destPort,message);
               Console.WriteLine($"... {msg.time} 0 < {msg.from} {msg.to} {msg.tok} {msg.pay}");
           }
       }

       public static void releaseMessage(string from,string to){
            while(Node.traffic.Count() > 0){
    
               MsgBlock msg = Node.traffic.Dequeue();
               string destPort = Node.confData[msg.to].ToString();
               string message = msg.ToString();
               DavidNet.sendData("http://localhost:"+destPort,message);
               Console.WriteLine($"... {msg.time} 0 < {msg.from} {msg.to} {msg.tok} {msg.pay}");
               if(String.Equals(msg.from,from) && String.Equals(msg.to,to)){
            //Console.WriteLine("111");
                return;
           }
       }
        
          }
        }

      
    

    public class NodeModule : NancyModule 
        {
        public NodeModule () 
            {
            //Get ("/", _ => "Hello World!");
        
            Get ("/greet/", _ => "Hello Anonymous!");
        
            Get ("/greet/{name}", x => {
                return string.Concat("Hello ", x.name);
                });    
            Post ("/", async x => {
            var reader = new StreamReader(this.Request.Body);
            
            var data = await reader.ReadToEndAsync();
            //var data = reader.ReadToEnd();
            //Console.WriteLine("info: "+ data.ToString());
            lock(Node.mylock){
            string[] data_list = data.ToString().Split();
            //Console.WriteLine(data_list[0]);
            if (data_list.Length == 3){
              //Console.WriteLine("666");
              DavidNet.releaseMessage(data_list[1],data_list[2]);
              return "233";
            }
            else if(String.Equals(data_list[2],"0")){
                Console.WriteLine($"... {data_list[0]}+ 0 > {data_list[1]} {data_list[2]} {data_list[3]} {data_list[4]}");
                //Console.WriteLine("The network size is " + data_list[4]);
                return "112";
            }

            

            try{
            //DavidNet.NoDelayTest(data_list);
            Console.WriteLine($"... {data_list[0]}+ 0 > {data_list[1]} {data_list[2]} {data_list[3]} {data_list[4]}");
            
              int curr_Time = Int32.Parse(data_list[0]) + DavidNet.getDelay(data_list[1],data_list[2]);
             //Console.WriteLine(curr_Time.ToString());
              data_list[0] = curr_Time.ToString();
              MsgBlock mes =  new MsgBlock(data_list);
              Node.traffic.Enqueue(mes);
              if (String.Equals(mes.tok, "1") || String.Equals(mes.tok, "3")){
                  Node.traffic.boundary = mes.logicalTime;

              }
             
              //Node.traffic.boundary = mes;
              //Console.WriteLine("234");
              DavidNet.DeliverMessage();
              //Console.WriteLine("out");
              
            }
            catch (Exception  excp){
                Console.WriteLine(excp.ToString());
            }
             // Console.WriteLine("go");
            }
                return "111";
            });
            
            }
        }
        
}




