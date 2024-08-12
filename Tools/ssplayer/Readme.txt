The System Shock Random Generator By Cless
------------------------------------------

Version: 1.0
Webpage: http://www.users.on.net/triforce/ssplayer.html
Webpage: https://wenchy.net/old/ssplayer.html


Usage
------

Run the program and follow the prompts on screen. Read the SSPlayer webpage
for more detailed info on the running of the program.


Command Line Options
--------------------
Note that the FULL path must be entered for filenames, or the utility will
fail (e.g. "SSPlayer.exe -extractall C:\XMI_Music_Files\Music.xmi")!

SSPLAYER  (filename.rsg (length (seed_value)))
or
SSPLAYER  (filename.mid/rmi)
or
SSPLAYER  (filename.xmi (sequence_number))

Options:

-?, -h, -h      - Displays this Information

-width=val      - Sets the visulization width to val. This value should be
                  smaller than the width of the Window. If the value is the
                  same or larger than the window width, the visulization will
                  scoll. This defauls to 79.

-visulize=val   - Sets the visulization update value to this count per quarter
                  note. The Default Value is 8 update per quarter note.

-drum           - Enable visulization of the drum track.

-hide           - Disable all Visulization.

-mt32           - Convert Songs that use Captial MT32 Tones to General MIDI

-extract        - Extract the Sequence from the File. This option does not work
                  with .RSG files

-extractall     - Extract all Sequences from the File. This option does not work
                  with .RSG files

NOTE that all options must begin with - and must be in lowercase

If the filename, length, seed value or sequence number are not specified you
will be prompted for them. 



Who Am I?
---------

I'm Cless. Cless is an abrieviation of Colourless, a name that I'm also known as.
Of course, those are my real name. My real name is Ryan Nunn. I'm a 21 year old
Australian, who loves System Shock and it's sequal.

email: triforce@merlin.net.au
ICQ#: 9421058
