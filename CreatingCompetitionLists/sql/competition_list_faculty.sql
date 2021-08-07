create table faculty
(
    id    bigint auto_increment
        primary key,
    Title varchar(50) null
);

INSERT INTO competition_list.faculty (id, Title) VALUES (1, 'ИСАУ');
INSERT INTO competition_list.faculty (id, Title) VALUES (2, 'ФЕИН');
INSERT INTO competition_list.faculty (id, Title) VALUES (3, 'ИФИ');
INSERT INTO competition_list.faculty (id, Title) VALUES (4, 'ФСГН');
INSERT INTO competition_list.faculty (id, Title) VALUES (8, 'Филиал «ДИНО»');
INSERT INTO competition_list.faculty (id, Title) VALUES (9, 'Филиал «Протвино»');