using System; 
using CommandLine; 

namespace MyLayoutManager
{
    public class Options
    {
        private string layoutname = string.Empty;        
    
        [Option('s', "savelayout",
        Default = false, 
        Required = false, 
        HelpText = "Tells the layout manager to save the current layout of open windows."
        )]
        public bool SaveLayout {get; set;}

        [Option('l', "loadlaout", 
        Default = false, 
        Required = false, 
        HelpText = "Tells the layout manager a saved layout of windows.")]
        public bool LoadLayout {get;set;}        
         

        [Option('n', "layoutname", 
        Default = ".\\LayoutConfig.json",  
        Required = false, 
        HelpText = "Save the current layout of open windows. Provide a name to save layout to a different config file.")]
        public String LayoutName 
        {
            get { return layoutname; }
            
        set
        {
            if(!value.EndsWith(".json"))            
                layoutname = value + ".json";             
            else
                layoutname = value;    

            layoutname = layoutname.Trim();      
        }}

       

        [Option('i', "interactive",
        Required = false,
        Default = false, 
        HelpText = "Cycles through open windows and allows you to choose which windows to save to a layout.")]
        public bool Interactive {get; set;}
    }
}