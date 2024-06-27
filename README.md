# tasktitan.NET

## syntax

`task add Read chapter 1 of GTD`
`task next`
`task done <ID>`
`task list`

## on startup

- if source directory contains .tasktitan folder
  - set that as config directory
- else if "%userprofile%\.config" contains .tasktitan folder
  - set that as config directory
- else throw exception
