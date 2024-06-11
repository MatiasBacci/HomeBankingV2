Descripción del Proyecto
HomeBankingV2 es una aplicación de banca en línea que permite a los clientes gestionar sus cuentas bancarias, tarjetas de crédito y débito, realizar transacciones y solicitar préstamos. La aplicación está diseñada para proporcionar una experiencia segura y eficiente utilizando tecnologías modernas para autenticación y gestión de datos.

Tecnologías Utilizadas
ASP.NET Core: Framework principal para construir la API web.
Entity Framework Core: ORM para interactuar con la base de datos.
Microsoft SQL Server: Base de datos relacional para almacenar la información.
Identity: Manejo de la autenticación y autorización.
Cookies: Manejo de sesiones del usuario.
BCrypt: Para el hash de contraseñas.

Funcionalidades

Registro y Logueo de Usuarios:
Los usuarios pueden registrarse proporcionando un correo electrónico y una contraseña.
Las contraseñas se almacenan de forma segura utilizando el hash de BCrypt.
Los usuarios pueden iniciar sesión utilizando su correo electrónico y contraseña.
Una vez autenticados, se utiliza una cookie para mantener la sesión del usuario.

Gestión de Cuentas Bancarias:
Crear nuevas cuentas bancarias.
Consultar las cuentas bancarias asociadas a un usuario.
Ver detalles de cada cuenta, como número de cuenta y sus respectivos movimientos de saldo.

Gestión de Tarjetas de Crédito y Débito:
Crear nuevas tarjetas de crédito y débito.
Consultar las tarjetas asociadas a un usuario.
Ver detalles de cada tarjeta, como número, tipo (crédito o débito), y fecha de vencimiento.

Transacciones:
Realizar transferencias entre cuentas, tanto propias como a terceros.
Consultar el historial de transacciones desde cada cuenta
Ver detalles de cada transacción, como monto, fecha, y cuentas involucradas.

Préstamos:
Solicitar nuevos préstamos.
Ver detalles de cada préstamo, como monto, tasa de interés, y plazo de pago.

Estructura del Proyecto
Capa de Controladores
Los controladores manejan las solicitudes HTTP y llaman a los servicios correspondientes para realizar las operaciones solicitadas.

Capa de Servicios
Los servicios contienen la lógica de negocio y se encargan de interactuar con los repositorios para realizar operaciones con la base de datos.

Capa de Repositorios
Los repositorios manejan las operaciones CRUD (Crear, Leer, Actualizar, Borrar) con la base de datos utilizando Entity Framework Core.
