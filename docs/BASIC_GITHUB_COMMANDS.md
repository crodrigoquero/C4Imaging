# GitHub Basic Commands

Add comments to every commit so that others can read your code more easily.

Be sure to commit your change every now and then so that others can track your changes more easily.

## How to commit your change

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
$ git commit -m "<YOUR NAME>: <TASK>" e.g. commit -m "Anndo: Add css code to style the buttons"
```

## How to use branch to collaborate
Make a new branch to work on each of your tasks, and then push it to GitHub and create a pull request once you have it done. The purpose of using branches is to avoid messing up with the master branch.

Update your master branch using:
```s
$ git pull origin master
```

Create a new branch using:
```javascript
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