# gui-widgets

`gui-widgets` is a project that provides GUI widgets designed to help command-line programs display data using graphical user interfaces (GUI).

## Install

To install `gui-widgets`, use the following command:

```
pip install --user git+https://github.com/gzj/gui-widgets.git
```

## Usage

Using Pipe to Get Data

```shell
#bash
echo "a \
b \
c \
d" | gk-list.py | xargs echo

#pwsh
echo "a `
b `
c `
d" | gk-list.py | ForEach-Object { echo $_ }
```

Using Arguments to Get Data

```shell
#bash
gk-list.py "a \
b \
c \
d" | xargs echo

#pwsh
gk-list.py "a `
b `
c `
d" | ForEach-Object { echo $_ }
```
