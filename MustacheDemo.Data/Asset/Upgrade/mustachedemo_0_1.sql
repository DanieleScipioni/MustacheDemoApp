CREATE TABLE [data](
    [id] TEXT PRIMARY KEY NOT NULL UNIQUE, 
    [content] TEXT);

CREATE TABLE [templates](
    [name] TEXT PRIMARY KEY NOT NULL UNIQUE, 
    [template] TEXT);

INSERT INTO [templates]
([name],[template])
VALUES
('First template' ,'Hello {{Name}}\nYou have just won {{Value}} {{Currency}}!\n{{#InCa}}\nWell, {{TaxedValue}} {{Currency}}, after taxes.\n{{/InCa}}\n{{#List}}\n{{.}}\n{{/List}}\n{{#d}}\n{{e}}: {{tt}}\n{{/d}});');
