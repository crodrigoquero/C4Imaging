# GitHub Basic Commands

This document is just a reminder about the most frequently used GitHub usedcommands; It does not explain every command in detail. If you are not familiar with GitHub and its commands then I recommend you have a look at the __GitHub documentation__.

## How to commit your change
__Remember__ to add comments to every commit so that others can read your code more easily and be sure to commit your changes every now and then so that others can keep track :+1:

Add the file(s) you have modified to the staging area using:
```s
$ git add . 
```
or
```s
$ git add <filename>
```
Commit and write what changes you have made in the commit message using:
```s
$ git commit -m "<YOUR NAME>: <TASK>" 
```

## How to use branch to collaborate
Make a new branch to work on each of your tasks, and then push it to GitHub and create a pull request once you have it done. The purpose of using branches is to avoid messing with the master branch.

Update your master branch using:
```s
$ git pull origin master
```

Create a new branch using:
```s
$ git branch <TASK NAME> 
```

Switch to the branch you just created using:
```s
$ git checkout <BRANCH NAME> 
```

After you complete the task, switch to your master branch using:
```s
$ git checkout master
```

Push your branch using:
```s
$ git push origin <BRANCH NAME>
```
