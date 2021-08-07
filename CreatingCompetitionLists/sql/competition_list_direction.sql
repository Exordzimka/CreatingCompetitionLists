create table direction
(
    id                 bigint auto_increment,
    Title              varchar(100) not null,
    faculty_id         bigint       null,
    short_title        varchar(50)  null,
    count_for_enrollee int          null,
    constraint direction_pk
        unique (id),
    constraint direction_faculty_id_fk
        foreign key (faculty_id) references faculty (id)
            on update cascade on delete cascade
);

INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (1, 'Программная инженерия', 1, 'ПРИНЖ', 65);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (2, 'Информатика и вычислительная техника', 1, 'ИВТ', 65);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (3, 'Информационные системы и технологии', 1, 'ИСТ', 25);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (4, 'Прикладная математика и информатика', 1, 'ПМИ', 15);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (5, 'Фундаментальная информатика и информационные технологии', 1, 'ФИИТ', 25);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (6, 'Прикладная информатика', 1, 'ПИ', 50);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (7, 'Автоматизация технологических процессов и производств', 1, 'АТПП', 15);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (8, 'Бизнес-информатика', 1, 'БИ', 10);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (9, 'Экономика', 1, 'Э', 0);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (10, 'Физика', 3, 'Ф', 15);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (11, 'Конструирование и технология электронных средств', 3, 'КТЭС', 27);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (12, 'Электроэнергитка и электротехника', 3, 'ЭЭ', 20);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (13, 'Ядерные физика и технологии(Радиобиология)', 3, 'ЯФТ(РБ)', 10);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (14, 'Ядерные физика и технологии(Физика ядерных реакций низких энергий)', 3, 'ЯФТ(ФЯРНЭ)', 10);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (15, 'Ядерные физика и технологии(Электроника и автоматика физических установок)', 3, 'ЯФТ(ЭАФУ)', 10);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (16, 'Авиастроение', 3, 'А', 15);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (17, 'Химия', 2, 'Х', 20);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (18, 'Химия, физика и механика материалов', 2, 'ХФММ', 15);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (19, 'Экология и природопользование', 2, 'ЭП', 20);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (20, 'Технология геологической разведки', 2, 'ТГР', 20);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (21, 'Психология', 4, 'П', 15);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (22, 'Клиническая психология', 4, 'КП', 25);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (23, 'Социология', 4, 'С', 20);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (24, 'Социальная работа', 4, 'СР', 12);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (25, 'Лингвистика', 4, 'Л', 15);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (26, 'Юриспруденция', 4, 'Ю', 10);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (45, 'Государственное и муниципальное управление', 4, 'ГМУ', 10);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (46, 'Информатика и вычислительная техника', 9, 'ИВТ', 20);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (47, 'Физика', 9, 'Ф', 15);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (48, 'Автоматизация технологических процессов и производств(АТПП)', 9, 'АТПП(АТПП)', 15);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (49, 'Автоматизация технологических процессов и производств(АКПЭУ)', 9, 'АТПП(АКПЭУ)', 10);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (50, 'Информатика и вычислительная техника', 8, 'ИВТ', 10);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (51, 'Прикладная информатика', 8, 'ПИ', 15);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (52, 'Строительство', 8, 'С', 15);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (53, 'Эксплуатация транспортно-технологических машин и комплексов', 8, 'ЭТТМП', 15);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (54, 'Экономика', 8, 'Э', 0);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (55, 'Менеджмент', 8, 'М', 0);
INSERT INTO competition_list.direction (id, Title, faculty_id, short_title, count_for_enrollee) VALUES (56, 'Государственное и муниципальное управление', 8, 'ГМУ', 0);