# IRC_Library
Simple Object-oriented C# IRC Client Library. Can be used for both bots and clients.

## Simple Client
Create simple IRC client.
This code works like telnet (after connecting to server) so you have to write full IRC commands but it is useful for testing.

### Code
```
Console.WriteLine("Starting...");

IRC irc = new IRC("<server>", 6667);

irc.RawMessageReceived += Console.WriteLine;

Console.CancelKeyPress += (sender, e) =>
{
    if (irc.Connected)
        irc.Close();
    Environment.Exit(1);
};

Console.WriteLine("Connecting...");

irc.Connect("<nick>", "<username>", "<real_name>");

Console.WriteLine("Connected");

while (true)
{
    string line = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(line))
        break;
    irc.SendRawMessage(line);
}

if (irc.Connected)
    irc.Close();
```
