namespace NReader.Storage.Sqlite.Migrations;

internal static class V1
{
    internal readonly static string Sql = @"
create table user(
    id integer primary key,
    created_at_utc text not null
);

create table source(
    id integer primary key,
    created_at_utc text not null
);

create table feed(
    id integer primary key,
    created_at_utc text not null,
    source_id integer not null,
    foreign key(source_id) references source(id)
);

create table article(
    id integer primary key,
    created_at_utc text not null,
    updated_at_utc text,
    feed_id integer not null,
    foreign key(feed_id) references feed(id)
);

create table history(
    id integer primary key,
    read_at_utc text not null,
    user_id integer not null,
    article_id integer not null,
    foreign key(user_id) references user(id),
    foreign key(article_id) references article(id)
);
";
}
