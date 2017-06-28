CREATE TABLE [data](
    [id] TEXT PRIMARY KEY NOT NULL UNIQUE, 
    [content] TEXT);

CREATE TABLE [templates](
    [name] TEXT PRIMARY KEY NOT NULL UNIQUE, 
    [template] TEXT);