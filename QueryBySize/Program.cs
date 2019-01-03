using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBySize
{
    class Program
    {
        private static List<string> errorList = new List<string>();

        private static string writeToTextFilePath;

        private static bool performQuerySizeGroupItem = false;

        private static int xLargesetFiles = 200;

        private static int counter = 0;

        private static string startFolder;
        
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            DateTime startTime = DateTime.Now;
            stopwatch.Start();

            writeToTextFilePath = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_QueryBySize.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            Console.WriteLine(string.Format("Start Time: {0}", startTime.ToString()));
            WriteToTextFile(string.Format("Start Time: {0}", startTime.ToString()));

            Console.WriteLine("writeToTextFilePath: {0}", writeToTextFilePath);
            WriteToTextFile(string.Format("writeToTextFilePath: {0}", writeToTextFilePath));

            if (ValidateReadInItems() == true)
            {
                Console.WriteLine("startFolder: {0}", startFolder);
                WriteToTextFile(string.Format("startFolder: {0}", startFolder));

                Console.WriteLine("xLargesetFiles: {0}", xLargesetFiles);
                WriteToTextFile(string.Format("xLargesetFiles: {0}", xLargesetFiles));

                QueryFilesBySize();

                Console.WriteLine(string.Empty);
                Console.WriteLine(string.Empty);
                Console.WriteLine(string.Empty);
                Console.WriteLine("Check the directory where the .exe is ran from to find the .txt  [ {0} ] file with the RUN information!", writeToTextFilePath);
                Console.WriteLine(string.Empty);
                Console.WriteLine(string.Empty);
            }

            // Stop timing.
            stopwatch.Stop();

            TimeSpan timeSpan = stopwatch.Elapsed;

            // Write result.
            Console.WriteLine(string.Empty);
            Console.WriteLine(string.Empty);
            WriteToTextFile(string.Format("Time elapsed: {0}", stopwatch.Elapsed));
            Console.WriteLine(string.Format("Time elapsed: {0}", stopwatch.Elapsed));
            Console.WriteLine(string.Empty);
            Console.WriteLine(string.Empty);
            Console.WriteLine(string.Format("Time: {0}h {1}m {2}s {3}ms", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds));
            WriteToTextFile(string.Format("Time: {0}h {1}m {2}s {3}ms", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds));
            Console.WriteLine(string.Empty);
            Console.WriteLine(string.Empty);
            Console.WriteLine(string.Empty);
            Console.WriteLine(string.Empty);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static bool ValidateReadInItems()
        {
            var list = new List<string>();
            using (StreamReader reader = new StreamReader(@"RequiredItems.txt"))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    list.Add(line);
                }
            }

            if (list.Count == 3)
            {
                startFolder = list.FirstOrDefault();

                if (Directory.Exists(startFolder) == false)
                {
                    Console.WriteLine(string.Format("Start Path [ {0} ] is NOT found.", startFolder));

                    return false;
                }

                int tryParseInt = 0;

                if (int.TryParse(list.Skip(1).Take(1).FirstOrDefault(), out tryParseInt) == true)
                {
                    xLargesetFiles = tryParseInt;
                }

                bool tryParseBool = false;

                if (bool.TryParse(list.LastOrDefault(), out tryParseBool) == true)
                {
                    performQuerySizeGroupItem = tryParseBool;
                }

                return true;
            }
            else
            {
                Console.WriteLine(string.Format("RequiredItems.txt is not formatted properly | Line 1 needs to be the start path of where to start the search ; Line 2 needs to be an integer value of the X number of files to return that are the largest ; Line 3 needs to be true or false based on if you want to perform the query size by group functionality"));

                return false;
            }
        }

        static IEnumerable<string> GetAllFiles(string path, string searchPattern)
        {
            return System.IO.Directory.EnumerateFiles(path, searchPattern).Union(
                System.IO.Directory.EnumerateDirectories(path).SelectMany(d =>
                {
                    try
                    {
                        return GetAllFiles(d, searchPattern);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        errorList.Add(string.Format("UnauthorizedAccessException: {0}", d));
                        WriteToTextFile(errorList.LastOrDefault());
                        Console.WriteLine("ErrorList Item: {0}", errorList.LastOrDefault());
                        return Enumerable.Empty<String>();
                    }
                }));
        }

        private static void WriteToTextFile(string text)
        {

            using (StreamWriter writer = new StreamWriter(writeToTextFilePath, true))
            {
                writer.WriteLine(text);
            }
        }

        public static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        private static void ProcessCounter(string forText)
        {
            counter++;
            //WriteToTextFileCounter(string.Format("Counter @ {0}: {1}", DateTime.Now.ToString(), counter));
            System.Diagnostics.Debug.WriteLine(string.Format("Counter-{0}: {1}", forText, counter));
            Console.WriteLine(string.Format("Counter-{0}: {1}", forText, counter));
            ClearLastLine();
        }

        private static void QueryFilesBySize()
        {
            Console.WriteLine("Start: {0}", DateTime.Now);
            WriteToTextFile(string.Format("Start: {0}", DateTime.Now));
            // Take a snapshot of the file system.  
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(startFolder);

            // This method assumes that the application has discovery permissions  
            // for all folders under the specified path.  
            var files = GetAllFiles(startFolder, "*.*");
           
            IList<System.IO.FileInfo> fileList = new List<System.IO.FileInfo>();

            Console.WriteLine("Counter reset for: getting file info @ {0}", DateTime.Now);
            counter = 0;
            foreach (var item in files)
            {
                try
                {
                    fileList.Add(new System.IO.FileInfo(item));
                }
                catch (System.IO.PathTooLongException)
                {
                    errorList.Add(string.Format("PathTooLongException: {0}", item));
                    WriteToTextFile(errorList.LastOrDefault());
                    Console.WriteLine("ErrorList Item: {0}", errorList.LastOrDefault());
                }

                ProcessCounter(" fileList.Add(...) ");
            }

            Console.WriteLine("fileList count: {0}", fileList.Count.ToString());
            WriteToTextFile(string.Format("fileList count: {0}", fileList.Count.ToString()));

            Console.WriteLine("Processing...  Please wait... ");
            //Return the size of the largest file  
            long maxSize =
                (from file in fileList
                 let len = GetFileLength(file)
                 select len)
                 .Max();

            WriteToTextFile(string.Format("The length of the largest file under {0} is {1}",
                startFolder, ConverToKbMbGb(maxSize)));

            Console.WriteLine("Return the size of the largest file  DONE @ {0}", DateTime.Now);
            WriteToTextFile(string.Format("Return the size of the largest file  DONE @ {0}", DateTime.Now));

            // Return the FileInfo object for the largest file  
            // by sorting and selecting from beginning of list  
            System.IO.FileInfo longestFile =
                (from file in fileList
                 let len = GetFileLength(file)
                 where len > 0
                 orderby len descending
                 select file)
                .First();

            WriteToTextFile(string.Format("The largest file under {0} is {1} with a length of {2}",
                                startFolder, longestFile.FullName, ConverToKbMbGb(longestFile.Length)));

            Console.WriteLine("Return the FileInfo object for the largest file    DONE @ {0}", DateTime.Now);
            WriteToTextFile(string.Format("Return the FileInfo object for the largest file    DONE @ {0}", DateTime.Now));

            //Return the FileInfo of the smallest file  
            System.IO.FileInfo smallestFile =
                (from file in fileList
                 let len = GetFileLength(file)
                 where len > 0
                 orderby len ascending
                 select file).First();

            WriteToTextFile(string.Format("The smallest file under {0} is {1} with a length of {2}",
                                startFolder, smallestFile.FullName, ConverToKbMbGb(smallestFile.Length)));

            Console.WriteLine("Return the FileInfo of the smallest file DONE @ {0}", DateTime.Now);
            WriteToTextFile(string.Format("Return the FileInfo of the smallest file DONE @ {0}", DateTime.Now));


            Console.WriteLine("Start - Return the FileInfos for the X largest file DONE @ {0}", DateTime.Now);
            WriteToTextFile(string.Format("Start - Return the FileInfos for the X largest file DONE @ {0}", DateTime.Now));

            //Return the FileInfos for the 10 largest files  
            // queryTenLargest is an IEnumerable<System.IO.FileInfo>  
            var queryXLargest =
                (from file in fileList
                 let len = GetFileLength(file)
                 orderby len descending
                 select file).Take(xLargesetFiles);

            WriteToTextFile(string.Format("The {0} largest files under {1} are:", xLargesetFiles, startFolder));

            Console.WriteLine("Counter reset for: queryXLargest @ {0}", DateTime.Now);
            counter = 0;
            foreach (var v in queryXLargest)
            {
                WriteToTextFile(string.Format("[{0}] | {1}: {2}", counter, ConverToKbMbGb(v.Length), v.FullName));
                ProcessCounter(" queryXLargest ");
            }

            Console.WriteLine("Start - Return the FileInfos for the X largest file DONE @ {0}", DateTime.Now);
            WriteToTextFile(string.Format("Start - Return the FileInfos for the X largest file DONE @ {0}", DateTime.Now));

            if (performQuerySizeGroupItem == true)
            {

                Console.WriteLine("Start - Group the files according to their size, leaving out files that are less than 200000 bytes.   DONE @ {0}", DateTime.Now);
                WriteToTextFile(string.Format("Start - Group the files according to their size, leaving out files that are less than 200000 bytes.   DONE @ {0}", DateTime.Now));
                // Group the files according to their size, leaving out  
                // files that are less than 200000 bytes.   
                var querySizeGroups =
                    from file in fileList
                    let len = GetFileLength(file)
                    where len > 0
                    group file by (len / 100000) into fileGroup
                    where fileGroup.Key >= 2
                    orderby fileGroup.Key descending
                    select fileGroup;

                Console.WriteLine("Counter reset for: querySizeGroups @ {0}", DateTime.Now);
                counter = 0;
                foreach (var filegroup in querySizeGroups)
                {
                    WriteToTextFile(string.Format(ConverToKbMbGb(filegroup.Key)));
                    foreach (var item in filegroup)
                    {
                        WriteToTextFile(string.Format("\t{0}: {1}", item.Name, ConverToKbMbGb(item.Length)));
                        ProcessCounter(" querySizeGroups ");
                    }
                }

                Console.WriteLine("Group the files according to their size, leaving out files that are less than 200000 bytes.   DONE @ {0}", DateTime.Now);
                WriteToTextFile(string.Format("Group the files according to their size, leaving out files that are less than 200000 bytes.   DONE @ {0}", DateTime.Now));
            }
            else
            {
                Console.WriteLine("performQuerySizeGroupItem is FALSE.   DONE @ {0}", DateTime.Now);
                WriteToTextFile(string.Format("performQuerySizeGroupItem is FALSE.   DONE @ {0}", DateTime.Now));
            }

            Console.WriteLine("End: {0}", DateTime.Now);
        }

        static string ConverToKbMbGb(long bytes)
        {
            string kilobytes1 = string.Format("{0} kilobytes", ConvertSize(bytes, "KB").ToString("0.00"));
            string megabytes1 = string.Format("{0} megabytes", ConvertSize(bytes, "MB").ToString("0.00"));
            string gigabytes1 = string.Format("{0} gigabytes", ConvertSize(bytes, "GB").ToString("0.00"));

            return string.Format("[ {0} | {1} | {2}]", kilobytes1, megabytes1, gigabytes1);
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        // This method is used to swallow the possible exception  
        // that can be raised when accessing the FileInfo.Length property.  
        // In this particular case, it is safe to swallow the exception.  
        static long GetFileLength(System.IO.FileInfo fi)
        {
            long retval;
            try
            {
                retval = fi.Length;
            }
            catch (System.IO.FileNotFoundException)
            {
                // If a file is no longer present,  
                // just add zero bytes to the total.  
                retval = 0;
            }
            return retval;
        }

        /// <summary>
        /// Function to convert the given bytes to either Kilobyte, Megabyte, or Gigabyte
        /// </summary>
        /// <param name="bytes">Double -> Total bytes to be converted</param>
        /// <param name="type">String -> Type of conversion to perform</param>
        /// <returns>Int32 -> Converted bytes</returns>
        /// <remarks></remarks>
        public static double ConvertSize(double bytes, string type)
        {
            try
            {
                const int CONVERSION_VALUE = 1024;
                //determine what conversion they want
                switch (type)
                {
                    case "BY":
                        //convert to bytes (default)
                        return bytes;
                    case "KB":
                        //convert to kilobytes
                        return (bytes / CONVERSION_VALUE);
                    case "MB":
                        //convert to megabytes
                        return (bytes / CalculateSquare(CONVERSION_VALUE));
                    case "GB":
                        //convert to gigabytes
                        return (bytes / CalculateCube(CONVERSION_VALUE));
                    default:
                        //default
                        return bytes;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Function to calculate the square of the provided number
        /// </summary>
        /// <param name="number">Int32 -> Number to be squared</param>
        /// <returns>Double -> THe provided number squared</returns>
        /// <remarks></remarks>
        public static double CalculateSquare(Int32 number)
        {
            return Math.Pow(number, 2);
        }


        /// <summary>
        /// Function to calculate the cube of the provided number
        /// </summary>
        /// <param name="number">Int32 -> Number to be cubed</param>
        /// <returns>Double -> THe provided number cubed</returns>
        /// <remarks></remarks>
        public static double CalculateCube(Int32 number)
        {
            return Math.Pow(number, 3);
        }
    }
}
