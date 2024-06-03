# Sample commands

task add I am a task that doesn't require quotations --due eom --wait friday

task add I am a task with a calculated wait --due wednesday --wait due-3d
task add I am a task with a calculated until --due wednesday --until due+4d

task modify -f due:eom -f scheduled:tomorrow -m due:1y -m scheduled:due-1d

## Built-in attributes

```
  text:           Task description text
  status:         Status of task - pending, completed, deleted, waiting
  project:        Project name
  due:            Due date
  recur:          Recurrence frequency
  until:          Expiration date of a task
  limit:          Desired number of rows in report, or 'page'
  wait:           Date until task becomes pending
  entry:          Date task was created
  end:            Date task was completed/deleted
  start:          Date task was started
  scheduled:      Date task is scheduled to start
  modified:       Date task was last modified
  depends:        Other tasks that this task depends upon
```

Date attributes should accept a string representation of a relative date i.e. `monday, eom, 1w (1 week)`

## Expressions

```
status:completed
project:Garden
status:pending and project:garden
due:eom
due:jan
due.none
scheduled.none
until
```
