using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using McMaster.Extensions.CommandLineUtils;

namespace b2c.Commands.IEF;

/// <summary>
/// Validate a TrustPolicy file against the schema.
/// </summary>
[Command(Name="validateSchema",Description = "validate a file against the TrustPolicy schema")]
public class ValidateSchema: BaseCommand
{
    [Option(LongName = "file",ShortName = "f",Description = "file to validate",ShowInHelpText = true)]
    public string pFile { get; set; }

    [Option(LongName = "schema",ShortName = "s",Description = "schema path",ShowInHelpText = true)]
    public string Schema { get; set; }

    public async Task OnExecuteAsync()
    {
        if (string.IsNullOrEmpty(pFile) || string.IsNullOrEmpty(Schema))
        {
            write("File and Schema are required parameters.");
            return;
        }

        if (!File.Exists(pFile))
        {
            write($"file {pFile} not found.");
            return;
        }
        
        verbose($"validating {pFile}...");

        var sw = Stopwatch.StartNew();
        //https://docs.microsoft.com/en-us/azure/active-directory-b2c/trustframeworkpolicy
        //https://docs.microsoft.com/en-us/dotnet/standard/data/xml/xml-schema-xsd-validation-with-xmlschemaset
        var settings = new XmlReaderSettings()
        {
            Async = true,
            ValidationType = ValidationType.Schema
        };
        settings.Schemas.Add("http://schemas.microsoft.com/online/cpim/schemas/2013/06", Schema);
        settings.ValidationEventHandler += SettingsOnValidationEventHandler;
        var doc = new XmlDocument();
        doc.Load(pFile);

        var reader = XmlReader.Create(pFile, settings);
        while (await reader.ReadAsync()){}

        verbose($"validation complete.");
        record(sw);
    }

    private void SettingsOnValidationEventHandler(object sender, ValidationEventArgs e)
    {
        var oc = console.ForegroundColor;

        if (e.Severity == XmlSeverityType.Warning)
        {
            console.ForegroundColor = ConsoleColor.Yellow;
            write($"WARNING: {e.Message}");
        }
        else if (e.Severity == XmlSeverityType.Error)
        {
            console.ForegroundColor = ConsoleColor.Red;
            write($"ERROR: {e.Message}");
        }
        else
        {
            write(e.Message);
        }

        console.ForegroundColor = oc;

    }

    public ValidateSchema(IConsole iconsole) : base(iconsole)
    {
    }
}
