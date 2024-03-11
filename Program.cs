//mycli bundle --output bundleFile.txt
using System;
using System.CommandLine;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

string[] lang = new string[] { "cs", "sql", "vb", "java", "py", "c", "cpp", "js", "html" };
string[] templang = new string[] { "c#", "sql", "vb", "java", "python", "c", "cpp", "js", "html", "all" };


var rootCommand = new RootCommand("Root command for file Bundler CLI");
var bundleCommand = new Command("bundle", "Bundle code files to a single file");
//var createRspCommand = new Command("create-response", "write the command");

//var outputOptionRsp = new Option<FileInfo>(new[] { "--output", "-o" }, "File path and name");

var outputOption = new Option<FileInfo>(new[] { "--output", "-o" }, "File path and name");
var languageOption = new Option<string[]>(new[] { "--language", "-l" }, "An option that must be one of the values of a static list")
    .FromAmong("c#", "sql", "vb", "java", "python", "c", "cpp", "js", "html", "all");
languageOption.IsRequired = true;
languageOption.AllowMultipleArgumentsPerToken = true;
var noteOption = new Option<bool>(new[] { "--note", "-n" }, "Copy the name and path of file to the new file");
var sortOption = new Option<string>(new[] { "--sort", "-s" }, "Sort the files").FromAmong("name", "type");
var remove_empty_lines = new Option<bool>(new[] { "--r", "-r" }, "Remove empty lines");
var authorOption = new Option<string>(new[] { "--author", "-a" }, "Write the author name");



rootCommand.AddCommand(bundleCommand);
//rootCommand.AddCommand(createRspCommand);

bundleCommand.AddOption(outputOption);
bundleCommand.AddOption(languageOption);
bundleCommand.AddOption(noteOption);
bundleCommand.AddOption(sortOption);
bundleCommand.AddOption(remove_empty_lines);
bundleCommand.AddOption(authorOption);

//createRspCommand.SetHandler((outputRsp) =>
//{
    
//}, outputOptionRsp);
bundleCommand.SetHandler((output, language, note, sort, remove, author) =>
{

    string outPutName = output.FullName;

    DirectoryInfo directoryPlace = new DirectoryInfo(Environment.CurrentDirectory);
    DirectoryInfo[] directories = directoryPlace.GetDirectories("*.", SearchOption.AllDirectories);
    List<FileInfo> fileToBund = new List<FileInfo>();

    try
    {
        foreach (DirectoryInfo d in directories)
        {
            if (d.FullName != "obj" && d.FullName != "bin")
            {
                FileInfo[] fileInfos = d.GetFiles();
                //סינון לפי שפות
                if (language[0] == "all")
                {

                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        
                        for (int i = 0; i < lang.Length; i++)
                        {
                            if (fileInfo.Extension == $".{lang[i]}")
                            {
                                
                                fileToBund.Add(fileInfo);

                            }
                        }

                    }
                }
                else
                {
                    bool flag = false;
                    int index = -1;
                    for (int i = 0; i < language.Length; i++)
                    {
                        for (int j = 0; j < templang.Length; j++)
                        {
                            
                            if (language[i] == templang[j])
                            {
                                flag = true;
                                index = j;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("The language is invalid try again");
                            }
                        }
                        if (!flag)
                        {
                            Console.WriteLine($"{language[i]} is inValid");
                        }
                        else
                        {
                            foreach (FileInfo fileInfo in fileInfos)
                            {
                                if (fileInfo.Extension == $".{lang[index]}")
                                {
                                    fileToBund.Add(fileInfo);
                                }
                            }
                        }

                    }
                }


            }
        }

    }
    catch { }
    if(sort== "type")
    {
        fileToBund.Sort((file1, file2) => string.Compare(file1.Extension, file2.Extension, StringComparison.Ordinal));
    }
    else if(sort=="name"||sort==null)
    {
        fileToBund = fileToBund.OrderBy(file => Path.GetFileName(file.FullName)).ToList();
    }

    if (author != null)
    {
        File.AppendAllText(output.FullName, "The creator of the file:" + author + "\n" + "\n");
    }

    foreach (FileInfo f in fileToBund)
    {
        try
        {
            if (note == true)
            {
                string path = f.FullName;
                File.AppendAllText(output.FullName, "sorce:" + path + "\n" + "\n");

            }
            string content = File.ReadAllText(f.FullName);
            if (remove)
            {
                string[] lines = content.Split('\n');
                content = string.Join("\n", lines.Where(line => !string.IsNullOrWhiteSpace(line)));
            }

            File.AppendAllText(output.FullName, content + "\n\n");
            content = "";
            File.AppendAllText(output.FullName, content + "\n" + "\n");
            content = "";
            Console.WriteLine(" copied successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

}, outputOption, languageOption, noteOption, sortOption, remove_empty_lines, authorOption);

rootCommand.InvokeAsync(args);





