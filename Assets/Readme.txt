Доброго времени суток!

Мне очень понравилось это техническое задание, было интересно поработать над его решением!
В целом, на мой взгляд, даже в таком узком скоупе получилось довольно неплохо, хотя, конечно же, на больших задачах в больших проектах можно удачнее применить использованные технологии (т.к. рамки проекта размыты, неизвестно "будет ли" расширяться функционал тех. задания и все такое).

При решении задачи воспользовался шиной событий (EventBusSystem), написана не мной, но дополнена мной, использую на всех своих проектах.
FPS Controller и DOTween с Asset Store.

Небольшие моменты:
* InteractableObject - можно было бы обойтись и без него, сделав базовый класс и наследуясь от него везде, где нужно, но ради демонстрации работы с UnityEvents оставил такой вариант (С интересным кастомным классом для передачи данных о предмете в руке)
* PlayerController - в нем поместилось много всего, можно было сделать проще, но взаимодействие с предметами получилось классным:)
* Все что связано с рюкзаком тоже можно было сделать попроще, а с UI для него я бы предпочел генерировать возможные ячейки на старте. Оставил как есть, но прокидывать референсы вручную - не очень. Лучше сделать один список возможных ячеек (Например в ScriptableObject), по нему же искать точки для визуального прикрепления предметов (или например сделать возможность класть предметы внутрь, просто скрывая их, без точки для присоединения). И уже с этого всего - строить UI, с фиксированными слотами, с пустыми слотами для любых предметов и прочее.
* Запрос совсем простой, скорее показательный:) У меня на сайте портфолио выложен проект, где я использую Mirror Networking, а ранее курировал проект с клиент-серверной архитектурой и все время работал с backend разработчиком
* Момент с сохранением состояния рюкзака: если имелось ввиду как сохранять содержимое из сессии в сессию, то лучше покажу код со своих проектов, где сохраняю все в json файл!