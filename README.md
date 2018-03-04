# Civil Discourse module

> Civil discourse is engagement in discourse (conversation) intended to enhance understanding.  
[Wikidedia](https://en.wikipedia.org/w/index.php?title=Civil_discourse&oldid=815158998)

The loss of Civil Discourse has been decried in a [wide](https://www.huffingtonpost.com/entry/the-importance-of-civil-discourse_us_59c5782be4b08d6615504261) [range](https://www.aacu.org/publications-research/periodicals/plea-civil-discourse-needed-academys-leadership) of [publications](https://www.wsj.com/articles/civil-discourse-in-decline-where-does-it-end-1496071276), and the [internet](http://www.latimes.com/opinion/readersreact/la-ol-le-civil-discourse-trump-internet-20170609-story.html) and [social media](https://www.technewsworld.com/story/85019.html) have been cited as a primary cause of this decline. 

The **admin/b** team took this as inspiration: can internet technology, and particularly the Sitecore Experience Platform and xConnect, help lead us back to using communication to enhance understanding rather than enflame passions? Our Civil Disourse module does not seek to ban people or prevent communication; rather it engages users by reminding them of the potential negative and positive impact of the words.  When users comment using inflamatory or hurtful language, the module responds by recommendeing alternative phrasings that will have a more constructive impact. xConnect is used to persist users comments, and to track negative interactions, enabling a 
down mechanism to temporarily block submissions from users who are engaging in a high volume of potentially abusive interactions.


## Dependencies

Before you can install Civil Discourse Module, you need to install the following items:

* Sitecore 9.0 rev. 171219 (Update-1) ([https://dev.sitecore.net/Downloads/Sitecore_Experience_Platform/90/Sitecore_Experience_Platform_90_Update1.aspx](https://dev.sitecore.net/Downloads/Sitecore_Experience_Platform/90/Sitecore_Experience_Platform_90_Update1.aspx))

## Installation

1. Install Sitecore package: CivilDiscourse-1.0.0.0.zip from the releases page: https://github.com/Sitecore-Hackathon/2018-admin-b/releases/tag/1.0.
2. Publish with subitems the following paths:
	1. /sitecore/templates/System/Settings
    2. /sitecore/layout/Layouts/Project/Layouts
    3. /sitecore/layout/Models/Project/CivilCommentsSection
    4. /sitecore/layout/Renderings/Project/CivilCommentSectionController
    5. /sitecore/content/Home/Global Components
    6. /sitecore/content/Home/Settings
    7. /sitecore/content/Home

## Using Civil Discourse Module

1. Setup: Creating questionable words

The Civil Discourse Module uses a repository of word items in Sitecore to scan each submitted comment for inappropriate language. These "questionable words" are stored in Sitecore, under /sitecore/content/Home/Global Components/Word Repository. Each word has the following fields:
 1. Word - text field. The word itself. This is case insensitive, and can also be a comma-separated list of similar words (e.g. "idiot, idiots"). 
 2. Severity Level - dropdown field, uses /sitecore/content/Home/Global Components/Severity Levels. This determines the color that the word is highlighted with; severe words show up in red, mild words show up in yellow, etc. You can add as many severity levels as you like, with any valid color ("green") or hex value ("#eeeeee) in the Warning Color field. 
 3. Custom Warning - text field. This field determines the message that is shown when the commenter hovers over the highlighted word, and should provide an explanation of why that word or phrase is discouraged.

The Civil Discourse Module uses global settings items to set the settings on the component and to retrieve and store user comments. The component settings item is location at /sitecore/content/Home/Settings/Settings. This item has the following fields:
 1. Flagged Words - multilist field. Select words from the Word Repo that you want to be flagged.
 2. Flagged Word Groups - multilist field, /sitecore/content/Home/Global Components/Word Groupings. A word grouping provides an easy way to group Word items together and select them all at once. A Word Grouping item also has a field for Warning Text, which will be used as the default warning text for every word in the group (overridden by Custom Warning Text on the Word item). For example, a warning group might be "Swears" or "Insults", and a standard warning can be applied across all insult words. 
 3. Intro Text - rich text field. This field displays optional text at the top of the comments section. This can be a simple heading, a conversation prompt, etc. 
 
The component has two parts, the comments list and the comment box. The comments are pulled from /sitecore/content/Home/Settings/Comments and displayed in date order, showing the username, the date posted, and the comment. 

![Forum](documentation/images/CivilDiscourceMain.PNG?raw=true "Forum")

When a user submits a comment, the module scans the comment for questionable language. The Settings item is used to aggregate the Flagged Words and the Flagged Word Groups into a list of Word items (word, severity color, warning text). For each word in this list, we do a case-insensitive search of the commment using regex. If no flagged words are found, the comment is submitted; otherwise, the user is shown a review message which asks them to review their comment and consider removing or changing the highlighted words. The user can hover over each highlighted word to see a tooltip with the warning message. 

![Review](documentation/images/CivilDiscourceCommentReview.PNG?raw=true "Review")

If the user clicks submit again without changing their comment, they will be prompted with a message asking if they're sure they want to submit; if they do edit their comment, the comment will be scanned again, and either submitted or returned with warnings again. In order to submit a comment that has flagged language, the user must go through the "are you sure" prompt. 

![Sure](documentation/images/CivilDiscourceAreYouSure.PNG?raw=true "Sure")


The optional cooldown field will disable the submit button for X seconds when a comment is returned with warnings. This prevents the user from submitting again immediately and encourages them to review and edit their comment. The cooldown can be as little as a few seconds, several minutes, or even hours, if the content editor is really sadistic. 


## Known Issues

Civil Discourse Module has several known issues. The notable issues are listed below:

1. When a Data Source is set on the Rendering, the component does not render, and does not show any helpful error messages. We have worked around this by using Global Settings, but this needs to be fixed so that unique settings can be applied to different pages.  

## Roadmap

1. Civil Discourse Module 1.1
 1. Use Datasource Item on rendering instead of Global Settings, so that different settings can be applied to different pages
 2. Incremental cooldowns - multiple the cooldown time on subsequent comment submissions, so that a user keeps getting warnings every time they try to submit one comment, they have to wait longer each time to try submitting again. 
 3. Block bad users - Track the number of warnings on the comments that each user submits, and enact temporary bans on users who can't behave (there would be some threshold for number of warnings within a certain time period)


## Developer Bootstrap

Want to develop for Civil Discourse Module? Great! To get started developing for CivilDiscourse, you must first do some things:

* Be awesome in powerful ways!

## About the Authors

Civil Discource Module was created by the [Velir](https://www.velir.com "Velir") [team admin/b](https://github.com/Sitecore-Hackathon/2018-admin-b/wiki/Team-admin-b "admin/b") (Dan Solovay, Ed Schwehm, Erica Stockwell-Alpert) as part of [2017 Sitecore Hackathon](http://www.sitecorehackathon.org/sitecore-hackathon-2018/ "Sitecore Hackathon 2018").


Team admin-b is brought to you by Daniel Solovay, Erica Stockwell-Alpert, and Ed Schwehm, sponsored by Velir.

![Hackathon Logo](documentation/images/hackathon.png?raw=true "Hackathon Logo")

----
## Video 

https://youtu.be/-W2qUEDpxxs

![powerful ways](https://i.imgur.com/hSLXsUQ.gif "Powerful Ways")
