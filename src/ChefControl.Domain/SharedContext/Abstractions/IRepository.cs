<<<<<<< HEAD
﻿using ChefControl.Domain.SharedContext.Abstractions;

namespace ChefControl.Domain.SharedContext.Persistence;
=======
﻿namespace ChefControl.Domain.SharedContext.Abstractions;
>>>>>>> 424443c (feat: Add tests for TaxId)

public interface IRepository<T> where T : IAggregateRoot;