# @schemaland/net-doc-plugin:toc

## Options

### input

- (string): Folder containing the documents.

### output

- (string): Folder to write the resulting toc.yml in.

### verbose

- (boolean): Show verbose messages.

### sequence

- (boolean): Use the .order files for TOC sequence. Format are raws of: filename-without-extension

### override

- (boolean): Use the .override files for TOC file name override. Format are raws of: filename-without-extension;Title you want

### index

- (boolean): Auto-generate a file index in each folder.

### ignore

- (boolean): Use the .ignore files for TOC directory ignore. Format are raws of directory names: directory-to-ignore

### multitoc

- (number): Generate multiple toc files for child folders down to a certain child depth, default is 0 (root only generation).

### skipInstall

- (boolean): Whether to skip installing dependencies.
