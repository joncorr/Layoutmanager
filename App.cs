using System.Text;
using PInvoke;

namespace MyLayoutManager
{
    public class App
    {
        public App(){}

        public string FileName { get; set; }
        public string LaunchArgs {get; set;}
        public string ProcessName {get; set;}
        public string WindowTitle { get; set; }
        public string ShortWindowTitle {get;set; }
        public User32.WINDOWPLACEMENT Placement { get; set; }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(); 
            sb.Append(string.Format("FilenName = {0}, ", FileName));
            sb.Append(string.Format("ProcessName = {0}, ", ProcessName)); 
            sb.Append(string.Format("LaunchArgs = {0}, ", LaunchArgs)); 
            sb.Append(string.Format("WindowTitle = {0}, ", WindowTitle));
            sb.Append(string.Format("Placement = {0}, ", Newtonsoft.Json.JsonConvert.SerializeObject(Placement))); 
            return sb.ToString();
        }
    }
}