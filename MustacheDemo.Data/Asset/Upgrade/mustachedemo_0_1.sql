CREATE TABLE [templates] (
    [name] TEXT PRIMARY KEY NOT NULL UNIQUE, 
    [template] TEXT
);

CREATE TABLE [data] (
    [id] TEXT PRIMARY KEY NOT NULL UNIQUE, 
    [content] TEXT
);

CREATE TABLE [template_data](
    [template_name] TEXT NOT NULL REFERENCES templates([name]), 
    [data_id] TEXT NOT NULL REFERENCES data([id]), 
    PRIMARY KEY([template_name], [data_id])
);

INSERT INTO [templates] ([name],[template])
VALUES
('Sample template' ,'Hello {{Name}}
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

INSERT INTO [data] ([id],[content])
VALUES ('Sample data','');
