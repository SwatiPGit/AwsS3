namespace ConsumeAwsS3Api;
public static class LogEvents
{
    public static void LogtoFile(string title, string message)
    {
        try 
        {
            // Create log folder
            string filePath = Directory.GetCurrentDirectory();
            string logFilesFolderPath = filePath + "\\" + "LogFiles";
            Directory.CreateDirectory(logFilesFolderPath);


            StreamWriter writer;
            string fileName = DateTime.Now.ToString("ddMMyyyy") + ".txt";
            logFilesFolderPath = Path.Combine(logFilesFolderPath, fileName);
            if (!File.Exists(logFilesFolderPath))
            {
                writer = new StreamWriter(logFilesFolderPath);
            }
            else
            {
                writer = File.AppendText(logFilesFolderPath);
            }

            writer.WriteLine("Log Entry");
            writer.WriteLine("{0} {1}",DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
            writer.WriteLine("Title: {0}", title);
            writer.WriteLine("Message: {0}", message);
            writer.WriteLine("---------------------------------------------------");
            writer.WriteLine("");
            writer.Close();

        } catch(Exception ex) { }
    }
}
