CREATE TABLE [data](
    [id] TEXT PRIMARY KEY NOT NULL UNIQUE, 
    [content] TEXT);

CREATE TABLE [templates](
    [name] TEXT PRIMARY KEY NOT NULL UNIQUE, 
    [template] TEXT);

INSERT INTO [templates]
([name],[template])
VALUES
('First template' ,'Hello {{Name}}
You have just won {{Value}} {{Currency}}!
{{#InCa}}
Well, {{TaxedValue}} {{Currency}}, after taxes.
{{/InCa}}
{{#List}}
{{.}}
{{/List}}
{{#d}}
{{e}}: {{tt}}
{{/d}}');
