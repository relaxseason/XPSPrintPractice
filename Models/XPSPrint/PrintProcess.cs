using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XPSPrintPractice.Models.XPSPrint
{
    public class PrintProcess
    {
        public PrintProcess()
        {
            log = string.Empty;
        }
        string log;

        public async Task<string> Print()
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var writer = new StreamWriter(ms))
                    using (var syncWriter = TextWriter.Synchronized(writer))
                    {
                        var printThread = new Thread(new ThreadStart(()=> PrintXPS(syncWriter)));
                        printThread.SetApartmentState(ApartmentState.STA);
                        await Task.Run(() => printThread.Start());
                        printThread.Join();
                        syncWriter.Flush();
    
                    }
                    using (var reader = new StreamReader(ms)){
                        log = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                return log;
            }
            return log;
        }

        void PrintXPS(TextWriter log)
        {
            // Create print server and print queue.
            var localPrintServer = new LocalPrintServer();
            PrintQueue defaultPrintQueue = LocalPrintServer.GetDefaultPrintQueue();

            // Prompt user to identify the directory, and then create the directory object.
            //Console.Write("Enter the directory containing the XPS files: ");

            var directoryPath = "D:\\hiro\\repo\\NoteBook\\NotebookFormatSVG";
            var dir = new DirectoryInfo(directoryPath);

            // If the user mistyped, end the thread and return to the Main thread.
            if (!dir.Exists)
            {
                log.WriteLine("There is no such directory.");
            }
            else
            {
                // If there are no XPS files in the directory, end the thread
                // and return to the Main thread.
                if (dir.GetFiles("*.xps").Length == 0)
                {
                    log.WriteLine("There are no XPS files in the directory.");
                }
                else
                {
                    log.WriteLine("\nJobs will now be added to the print queue.");
                    log.WriteLine("If the queue is not paused and the printer is working, jobs will begin printing.");

                    // Batch process all XPS files in the directory.
                    foreach (FileInfo f in dir.GetFiles("*.xps"))
                    {
                        String nextFile = directoryPath + "\\" + f.Name;
                        log.WriteLine("Adding {0} to queue.", nextFile);

                        try
                        {
                            // Print the Xps file while providing XPS validation and progress notifications.
                            PrintSystemJobInfo xpsPrintJob = defaultPrintQueue.AddJob(f.Name, nextFile, false);
                        }
                        catch (PrintJobException e)
                        {
                            log.WriteLine("\n\t{0} could not be added to the print queue.", f.Name);
                            if (e.InnerException.Message == "File contains corrupted data.")
                            {
                                log.WriteLine("\tIt is not a valid XPS file. Use the isXPS Conformance Tool to debug it.");
                            }
                            log.WriteLine("\tContinuing with next XPS file.\n");
                        }
                    }// end for each XPS file
                }//end if there are no XPS files in the directory
            }//end if the directory does not exist

            //log.WriteLine("Press Enter to end program.");
            //log.ReadLine();
        }


    }
}
